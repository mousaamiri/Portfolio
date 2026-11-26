import { useCallback, useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { Icon } from "../lib/icons";
import { api } from "../lib/api";
import { usePageMeta } from "../lib/hooks";
import { RESOURCES } from "./config";
import { useToast } from "./toast";

function StatusBadge({ published, kind }) {
  const label = kind === "published" ? (published ? "Published" : "Draft") : published ? "Active" : "Inactive";
  return (
    <span className={`admin-status ${published ? "admin-status--published" : "admin-status--draft"}`}>
      <span className="admin-status-dot"></span>
      {label}
    </span>
  );
}

export default function AdminList() {
  const { resource } = useParams();
  const cfg = RESOURCES[resource];
  const { showToast } = useToast();

  usePageMeta(cfg ? `Admin — ${cfg.title}` : "Admin");

  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(true);
  const [version, setVersion] = useState(0);

  const reload = useCallback(() => setVersion((v) => v + 1), []);

  useEffect(() => {
    if (!cfg) return;
    let active = true;
    setLoading(true);
    api
      .adminList(cfg.resource, cfg.listLang || "en")
      .then((data) => {
        if (active) setRows(data);
      })
      .catch(() => {
        if (active) setRows([]);
      })
      .finally(() => {
        if (active) setLoading(false);
      });
    return () => {
      active = false;
    };
  }, [cfg, version]);

  if (!cfg) {
    return <div className="admin-empty"><p>Unknown resource.</p></div>;
  }

  const onDelete = async (row) => {
    if (!window.confirm("Delete this item? This cannot be undone.")) return;
    const ok = await api.adminDelete(cfg.resource, row.id);
    showToast(ok ? "Deleted." : "Could not delete the item.", ok ? "success" : "error");
    if (ok) reload();
  };

  return (
    <>
      <div className="admin-page-header">
        <div>
          <h1 className="admin-page-title">{cfg.title}</h1>
          <p className="admin-page-subtitle">{cfg.subtitle}</p>
        </div>
        <Link to={`/Admin/${resource}/new`} className="admin-btn admin-btn--primary">
          <Icon name="plus" size={16} />
          <span>{cfg.createLabel}</span>
        </Link>
      </div>

      {loading ? (
        <div className="admin-empty"><p>Loading…</p></div>
      ) : rows.length === 0 ? (
        <div className="admin-empty">
          <p>Nothing here yet.</p>
          <Link to={`/Admin/${resource}/new`} className="admin-btn admin-btn--primary admin-btn--sm">Create the first one</Link>
        </div>
      ) : (
        <table className="admin-table">
          <thead>
            <tr>
              {cfg.columns.map((c) => (
                <th key={c.label}>{c.label}</th>
              ))}
              <th>Status</th>
              <th style={{ textAlign: "right" }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((row) => {
              const published = cfg.status ? cfg.status(row) : true;
              return (
                <tr key={row.id}>
                  {cfg.columns.map((c, i) => (
                    <td key={c.label} className={i === 0 ? "admin-cell-strong" : ""}>
                      {c.value(row)}
                    </td>
                  ))}
                  <td>
                    <StatusBadge published={published} kind={cfg.statusKind} />
                  </td>
                  <td style={{ textAlign: "right", whiteSpace: "nowrap" }}>
                    <Link to={`/Admin/${resource}/${row.id}/edit`} className="admin-btn admin-btn--ghost admin-btn--sm">Edit</Link>
                    <button type="button" className="admin-btn admin-btn--danger admin-btn--sm" onClick={() => onDelete(row)}>Delete</button>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      )}
    </>
  );
}
