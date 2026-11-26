import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Icon } from "../lib/icons";
import { api } from "../lib/api";
import { usePageMeta } from "../lib/hooks";
import { RESOURCES, buildPayload, initialValues } from "./config";
import { useToast } from "./toast";

function FieldInput({ field, value, onChange, rtl }) {
  const inputClass = `admin-form-input${rtl ? " admin-form-input--rtl" : ""}`;
  const dir = rtl ? "rtl" : undefined;
  const set = (v) => onChange(field.name, v);

  if (field.type === "textarea") {
    return (
      <textarea
        id={field.name}
        className={`admin-form-textarea${rtl ? " admin-form-input--rtl" : ""}`}
        dir={dir}
        rows={field.rows || 4}
        value={value ?? ""}
        onChange={(e) => set(e.target.value)}
      />
    );
  }

  if (field.type === "select") {
    return (
      <select id={field.name} className={inputClass} value={value ?? ""} onChange={(e) => set(e.target.value)}>
        {field.options.map((o) => (
          <option key={o.value} value={o.value}>{o.label}</option>
        ))}
      </select>
    );
  }

  const type = field.type === "number" ? "number" : field.type === "date" ? "date" : "text";
  return (
    <input
      id={field.name}
      type={type}
      className={inputClass}
      dir={dir}
      placeholder={field.placeholder}
      value={value ?? ""}
      onChange={(e) => set(e.target.value)}
    />
  );
}

function FieldGroup({ field, value, onChange, rtl }) {
  if (field.type === "checkbox") {
    return (
      <label className="admin-check">
        <input type="checkbox" checked={!!value} onChange={(e) => onChange(field.name, e.target.checked)} />
        <span>{field.label}</span>
      </label>
    );
  }
  return (
    <div className="admin-form-group">
      <label className="admin-form-label" htmlFor={field.name}>{field.label}</label>
      <FieldInput field={field} value={value} onChange={onChange} rtl={rtl} />
      {field.hint && <span className="admin-form-hint">{field.hint}</span>}
    </div>
  );
}

function FieldSection({ fields, values, onChange, rtl, namePrefix = "" }) {
  const checks = fields.filter((f) => f.type === "checkbox");
  const inputs = fields.filter((f) => f.type !== "checkbox");

  const rows = [];
  let current = [];
  inputs.forEach((f) => {
    if (f.type === "textarea") {
      if (current.length) {
        rows.push({ type: "row", fields: current });
        current = [];
      }
      rows.push({ type: "single", field: f });
    } else {
      current.push(f);
      if (current.length === 2) {
        rows.push({ type: "row", fields: current });
        current = [];
      }
    }
  });
  if (current.length) rows.push({ type: "row", fields: current });

  return (
    <>
      {rows.map((r, i) =>
        r.type === "row" ? (
          <div className="admin-form-row" key={i}>
            {r.fields.map((f) => (
              <FieldGroup key={f.name} field={{ ...f, name: namePrefix + f.name }} value={values[namePrefix + f.name]} onChange={onChange} rtl={rtl} />
            ))}
          </div>
        ) : (
          <FieldGroup key={i} field={{ ...r.field, name: namePrefix + r.field.name }} value={values[namePrefix + r.field.name]} onChange={onChange} rtl={rtl} />
        )
      )}
      {checks.length > 0 && (
        <div className="admin-form-checks">
          {checks.map((f) => (
            <FieldGroup key={f.name} field={{ ...f, name: namePrefix + f.name }} value={values[namePrefix + f.name]} onChange={onChange} />
          ))}
        </div>
      )}
    </>
  );
}

export default function EntityForm() {
  const { resource, id } = useParams();
  const cfg = RESOURCES[resource];
  const isEdit = Boolean(id);
  const navigate = useNavigate();
  const { showToast } = useToast();

  const singular = cfg ? cfg.title.replace(/s$/, "") : "Item";
  usePageMeta(cfg ? `Admin — ${isEdit ? "Edit" : "New"} ${singular}` : "Admin");

  const [values, setValues] = useState(() => (cfg ? initialValues(cfg, null, null) : {}));
  const [loading, setLoading] = useState(isEdit);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!cfg) return;
    if (!isEdit) {
      setValues(initialValues(cfg, null, null));
      setLoading(false);
      return;
    }
    let active = true;
    setLoading(true);
    const load = async () => {
      const en = await api.adminGet(cfg.resource, id, cfg.flat ? cfg.listLang || "fa" : "en");
      const fa = cfg.flat ? null : await api.adminGet(cfg.resource, id, "fa");
      if (!active) return;
      if (!en) {
        setError("Could not load this item.");
        setLoading(false);
        return;
      }
      setValues(initialValues(cfg, en, fa));
      setLoading(false);
    };
    load().catch(() => {
      if (active) {
        setError("Could not load this item.");
        setLoading(false);
      }
    });
    return () => {
      active = false;
    };
  }, [cfg, id, isEdit]);

  if (!cfg) {
    return <div className="admin-empty"><p>Unknown resource.</p></div>;
  }

  const onChange = (name, value) => setValues((v) => ({ ...v, [name]: value }));

  const validate = () => {
    const required = cfg.flat
      ? cfg.flatFields.filter((f) => f.required)
      : [...cfg.base.filter((f) => f.required), ...cfg.translation.filter((f) => f.required).map((f) => ({ ...f, name: `en_${f.name}` }))];
    for (const f of required) {
      if (String(values[f.name] ?? "").trim() === "") {
        return `${f.label} is required.`;
      }
    }
    return "";
  };

  const onSubmit = async (e) => {
    e.preventDefault();
    const validationError = validate();
    if (validationError) {
      setError(validationError);
      return;
    }
    setError("");
    setSaving(true);
    const payload = buildPayload(cfg, values);
    try {
      const ok = isEdit ? await api.adminUpdate(cfg.resource, id, payload) : await api.adminCreate(cfg.resource, payload);
      if (!ok) {
        setError(`Could not ${isEdit ? "update" : "create"} this item.`);
        return;
      }
      showToast(`${singular} ${isEdit ? "updated" : "created"}.`);
      navigate(`/Admin/${resource}`);
    } catch {
      setError("Something went wrong. Please try again.");
    } finally {
      setSaving(false);
    }
  };

  const title = `${isEdit ? "Edit" : "New"} ${singular}`;

  return (
    <>
      <div className="admin-page-header">
        <div>
          <h1 className="admin-page-title">{title}</h1>
          <p className="admin-page-subtitle">{isEdit ? values.slug || "" : `Add a ${singular.toLowerCase()}`}</p>
        </div>
        <Link to={`/Admin/${resource}`} className="admin-btn admin-btn--ghost">Back to list</Link>
      </div>

      {loading ? (
        <div className="admin-empty"><p>Loading…</p></div>
      ) : (
        <form className="admin-form" onSubmit={onSubmit}>
          {error && <div className="admin-form-error" role="alert">{error}</div>}

          {cfg.flat ? (
            <FieldSection fields={cfg.flatFields} values={values} onChange={onChange} />
          ) : (
            <>
              <h3 className="admin-form-section">Details</h3>
              <FieldSection fields={cfg.base} values={values} onChange={onChange} />

              <h3 className="admin-form-section">English</h3>
              <FieldSection fields={cfg.translation} values={values} onChange={onChange} namePrefix="en_" />

              <h3 className="admin-form-section">Persian (optional)</h3>
              <FieldSection fields={cfg.translation} values={values} onChange={onChange} namePrefix="fa_" rtl />
            </>
          )}

          <div className="admin-form-actions">
            <Link to={`/Admin/${resource}`} className="admin-btn admin-btn--ghost">Cancel</Link>
            <button type="submit" className="admin-btn admin-btn--primary" disabled={saving}>
              {saving ? "Saving…" : isEdit ? "Save changes" : `Create ${singular.toLowerCase()}`}
            </button>
          </div>
        </form>
      )}
    </>
  );
}
