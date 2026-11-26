import { useCallback, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { api } from "../lib/api";
import { usePageMeta } from "../lib/hooks";
import { useToast } from "./toast";

export function formatDate(value) {
  if (!value) return "—";
  const d = new Date(value);
  if (Number.isNaN(d.getTime())) return "—";
  const pad = (n) => String(n).padStart(2, "0");
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())} ${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

export default function Messages() {
  usePageMeta("Admin — Messages");
  const { showToast } = useToast();
  const [messages, setMessages] = useState([]);
  const [loading, setLoading] = useState(true);
  const [version, setVersion] = useState(0);

  const reload = useCallback(() => setVersion((v) => v + 1), []);

  useEffect(() => {
    let active = true;
    setLoading(true);
    api
      .adminMessages()
      .then((data) => {
        if (active) setMessages([...data].sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt)));
      })
      .catch(() => {
        if (active) setMessages([]);
      })
      .finally(() => {
        if (active) setLoading(false);
      });
    return () => {
      active = false;
    };
  }, [version]);

  const onDelete = async (m) => {
    if (!window.confirm("Delete this message?")) return;
    const ok = await api.adminDeleteMessage(m.id);
    showToast(ok ? "Message deleted." : "Could not delete the message.", ok ? "success" : "error");
    if (ok) reload();
  };

  return (
    <>
      <div className="admin-page-header">
        <div>
          <h1 className="admin-page-title">Messages</h1>
          <p className="admin-page-subtitle">Contact-form submissions</p>
        </div>
      </div>

      {loading ? (
        <div className="admin-empty"><p>Loading…</p></div>
      ) : messages.length === 0 ? (
        <div className="admin-empty"><p>No messages yet.</p></div>
      ) : (
        <table className="admin-table">
          <thead>
            <tr>
              <th>From</th>
              <th>Subject</th>
              <th>Received</th>
              <th>Status</th>
              <th style={{ textAlign: "right" }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {messages.map((m) => (
              <tr key={m.id} style={m.isRead ? undefined : { fontWeight: 600 }}>
                <td className="admin-cell-strong">
                  {m.name}
                  <div className="admin-cell-sub" style={{ fontWeight: 400 }}>{m.email}</div>
                </td>
                <td>{m.subject || "—"}</td>
                <td>{formatDate(m.createdAt)}</td>
                <td>
                  <span className={`admin-status ${m.isRead ? "admin-status--draft" : "admin-status--published"}`}>
                    <span className="admin-status-dot"></span>
                    {m.isRead ? "Read" : "Unread"}
                  </span>
                </td>
                <td style={{ textAlign: "right", whiteSpace: "nowrap" }}>
                  <Link to={`/Admin/Messages/${m.id}`} className="admin-btn admin-btn--ghost admin-btn--sm">View</Link>
                  <button type="button" className="admin-btn admin-btn--danger admin-btn--sm" onClick={() => onDelete(m)}>Delete</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </>
  );
}
