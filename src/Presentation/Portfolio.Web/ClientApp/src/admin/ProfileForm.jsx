import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { api } from "../lib/api";
import { usePageMeta } from "../lib/hooks";
import { useToast } from "./toast";

const BASE_FIELDS = [
  { name: "email", label: "Email", required: true },
  { name: "githubUrl", label: "GitHub URL" },
  { name: "linkedinUrl", label: "LinkedIn URL" },
  { name: "instagramUrl", label: "Instagram URL" },
  { name: "telegramUrl", label: "Telegram URL" },
  { name: "twitterUrl", label: "Twitter URL" },
  { name: "websiteUrl", label: "Website URL" },
  { name: "resumeUrlEn", label: "Resume URL (EN)" },
  { name: "resumeUrlFa", label: "Resume URL (FA)" },
  { name: "portraitUrl", label: "Portrait URL" },
  { name: "learningDate", label: "Learning date", placeholder: "Jul 6, 2026" },
  { name: "phone", label: "Phone" },
  { name: "countryCode", label: "Country code", placeholder: "IR" },
];

const TRANSLATION_FIELDS = [
  { name: "fullName", label: "Full name", required: true },
  { name: "jobTitle", label: "Job title", required: true },
  { name: "tagline", label: "Tagline" },
  { name: "bio", label: "Bio", type: "textarea", rows: 4 },
  { name: "learningTitle", label: "Learning title" },
  { name: "learningDesc", label: "Learning description" },
  { name: "roleBadge", label: "Role badge", placeholder: "Software Engineer" },
  { name: "experienceBadge", label: "Experience badge", placeholder: "3+ Years" },
  { name: "degreeBadge", label: "Degree badge" },
  { name: "portraitAlt", label: "Portrait alt" },
  { name: "location", label: "Location", placeholder: "Tehran" },
  { name: "country", label: "Country", placeholder: "Iran" },
];

function Field({ field, value, onChange, rtl }) {
  const inputClass = `admin-form-input${rtl ? " admin-form-input--rtl" : ""}`;
  const dir = rtl ? "rtl" : undefined;
  return (
    <div className="admin-form-group">
      <label className="admin-form-label" htmlFor={field.name}>{field.label}</label>
      {field.type === "textarea" ? (
        <textarea
          id={field.name}
          className={`admin-form-textarea${rtl ? " admin-form-input--rtl" : ""}`}
          dir={dir}
          rows={field.rows || 4}
          value={value ?? ""}
          onChange={(e) => onChange(field.name, e.target.value)}
        />
      ) : (
        <input
          id={field.name}
          className={inputClass}
          dir={dir}
          placeholder={field.placeholder}
          value={value ?? ""}
          onChange={(e) => onChange(field.name, e.target.value)}
        />
      )}
    </div>
  );
}

function Grid({ fields, values, onChange, rtl, prefix = "" }) {
  return (
    <>
      {fields.map((f) => (
        <Field key={f.name} field={{ ...f, name: prefix + f.name }} value={values[prefix + f.name]} onChange={onChange} rtl={rtl} />
      ))}
    </>
  );
}

export default function ProfileForm() {
  usePageMeta("Admin — Profile");
  const { showToast } = useToast();
  const [values, setValues] = useState({});
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    let active = true;
    Promise.all([api.getProfile("en"), api.getProfile("fa")])
      .then(([en, fa]) => {
        if (!active) return;
        const next = {};
        BASE_FIELDS.forEach((f) => {
          next[f.name] = en?.[f.name] ?? "";
        });
        TRANSLATION_FIELDS.forEach((f) => {
          next[`en_${f.name}`] = en?.[f.name] ?? "";
          next[`fa_${f.name}`] = fa?.[f.name] ?? "";
        });
        setValues(next);
      })
      .catch(() => {
        if (active) setError("Could not load the profile.");
      })
      .finally(() => {
        if (active) setLoading(false);
      });
    return () => {
      active = false;
    };
  }, []);

  const onChange = (name, value) => setValues((v) => ({ ...v, [name]: value }));

  const onSubmit = async (e) => {
    e.preventDefault();
    setError("");

    if (!String(values.email ?? "").trim() || !String(values.en_fullName ?? "").trim() || !String(values.en_jobTitle ?? "").trim()) {
      setError("Email, full name and job title are required.");
      return;
    }

    const payload = {};
    BASE_FIELDS.forEach((f) => {
      payload[f.name] = values[f.name] ?? "";
    });

    const en = { languageCode: "en" };
    TRANSLATION_FIELDS.forEach((f) => {
      en[f.name] = values[`en_${f.name}`] ?? "";
    });
    const translations = [en];

    const hasFa = TRANSLATION_FIELDS.some((f) => String(values[`fa_${f.name}`] ?? "").trim() !== "");
    if (hasFa) {
      const fa = { languageCode: "fa" };
      TRANSLATION_FIELDS.forEach((f) => {
        fa[f.name] = values[`fa_${f.name}`] ?? "";
      });
      translations.push(fa);
    }
    payload.translations = translations;

    setSaving(true);
    try {
      const ok = await api.adminUpsertProfile(payload);
      if (ok) {
        showToast("Profile saved.");
      } else {
        setError("Could not save the profile.");
      }
    } catch {
      setError("Could not save the profile.");
    } finally {
      setSaving(false);
    }
  };

  return (
    <>
      <div className="admin-page-header">
        <div>
          <h1 className="admin-page-title">Profile</h1>
          <p className="admin-page-subtitle">Home hero + contact identity (single record)</p>
        </div>
      </div>

      {loading ? (
        <div className="admin-empty"><p>Loading…</p></div>
      ) : (
        <form className="admin-form" onSubmit={onSubmit}>
          {error && <div className="admin-form-error" role="alert">{error}</div>}

          <h3 className="admin-form-section">Contact &amp; links</h3>
          <div className="admin-form-row">
            <Field field={BASE_FIELDS[0]} value={values.email} onChange={onChange} />
          </div>
          <div className="admin-form-row">
            {BASE_FIELDS.slice(1, 4).map((f) => (
              <Field key={f.name} field={f} value={values[f.name]} onChange={onChange} />
            ))}
          </div>
          <div className="admin-form-row">
            {BASE_FIELDS.slice(4, 7).map((f) => (
              <Field key={f.name} field={f} value={values[f.name]} onChange={onChange} />
            ))}
          </div>
          <div className="admin-form-row">
            {BASE_FIELDS.slice(7, 10).map((f) => (
              <Field key={f.name} field={f} value={values[f.name]} onChange={onChange} />
            ))}
          </div>
          <div className="admin-form-row">
            {BASE_FIELDS.slice(10, 13).map((f) => (
              <Field key={f.name} field={f} value={values[f.name]} onChange={onChange} />
            ))}
          </div>

          <h3 className="admin-form-section">English</h3>
          <div className="admin-form-row">
            <Grid fields={TRANSLATION_FIELDS.slice(0, 2)} values={values} onChange={onChange} prefix="en_" />
          </div>
          <Grid fields={TRANSLATION_FIELDS.slice(2, 4)} values={values} onChange={onChange} prefix="en_" />
          <div className="admin-form-row">
            <Grid fields={TRANSLATION_FIELDS.slice(4, 6)} values={values} onChange={onChange} prefix="en_" />
          </div>
          <div className="admin-form-row">
            <Grid fields={TRANSLATION_FIELDS.slice(6, 9)} values={values} onChange={onChange} prefix="en_" />
          </div>
          <div className="admin-form-row">
            <Grid fields={TRANSLATION_FIELDS.slice(9, 12)} values={values} onChange={onChange} prefix="en_" />
          </div>

          <h3 className="admin-form-section">Persian (optional)</h3>
          <div className="admin-form-row">
            <Grid fields={TRANSLATION_FIELDS.slice(0, 2)} values={values} onChange={onChange} prefix="fa_" rtl />
          </div>
          <Grid fields={TRANSLATION_FIELDS.slice(2, 4)} values={values} onChange={onChange} prefix="fa_" rtl />
          <div className="admin-form-row">
            <Grid fields={TRANSLATION_FIELDS.slice(4, 6)} values={values} onChange={onChange} prefix="fa_" rtl />
          </div>
          <div className="admin-form-row">
            <Grid fields={TRANSLATION_FIELDS.slice(6, 9)} values={values} onChange={onChange} prefix="fa_" rtl />
          </div>
          <div className="admin-form-row">
            <Grid fields={TRANSLATION_FIELDS.slice(9, 12)} values={values} onChange={onChange} prefix="fa_" rtl />
          </div>

          <div className="admin-form-actions">
            <Link to="/Admin" className="admin-btn admin-btn--ghost">Cancel</Link>
            <button type="submit" className="admin-btn admin-btn--primary" disabled={saving}>
              {saving ? "Saving…" : "Save profile"}
            </button>
          </div>
        </form>
      )}
    </>
  );
}
