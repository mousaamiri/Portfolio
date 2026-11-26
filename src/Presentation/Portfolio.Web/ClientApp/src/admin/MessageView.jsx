import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { api } from "../lib/api";
import { usePageMeta } from "../lib/hooks";
import { useToast } from "./toast";
import { formatDate } from "./Messages";

export default function MessageView() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { showToast } = useToast();
  usePageMeta("Admin — Message");

  const [message, setMessage] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let active = true;
    api
      .adminMessage(id)
      .then((data) => {
        if (active) setMessage(data);
      })
      .catch(() => {
        if (active) setMessage(null);
      })
      .finally(() => {
        if (active) setLoading(false);
      });
    return () => {
      active = false;
    };
  }, [id]);

  if (loading) return <div className="admin-empty"><p>Loading…</p></div>;
  if (!message) return <div className="admin-empty"><p>Message not found.</p></div>;

  const markRead = async () => {
    const ok = await api.adminMarkMessageRead(message.id);
    if (ok) {
      setMessage((m) => ({ ...m, isRead: true }));
      showToast("Marked as read.");
    } else {
      showToast("Could not update the message.", "error");
    }
  };

  const remove = async () => {
    if (!window.confirm("Delete this message?")) return;
    const ok = await api.adminDeleteMessage(message.id);
    if (ok) {
      showToast("Message deleted.");
      navigate("/Admin/Messages");
    } else {
      showToast("Could not delete the message.", "error");
    }
  };

  return (
    <>
      <div className="admin-page-header">
        <div>
          <h1 className="admin-page-title">{message.name}</h1>
          <p className="admin-page-subtitle">{message.email} · {formatDate(message.createdAt)}</p>
        </div>
        <Link to="/Admin/Messages" className="admin-btn admin-btn--ghost">Back to inbox</Link>
      </div>

      <div className="admin-form" style={{ maxWidth: 760 }}>
        {message.phone && (
          <div className="admin-form-group">
            <span className="admin-form-label">Phone</span>
            <p><a href={`tel:${message.phone}`} dir="ltr">{message.phone}</a></p>
          </div>
        )}
        {message.subject && (
          <div className="admin-form-group">
            <span className="admin-form-label">Subject</span>
            <p>{message.subject}</p>
          </div>
        )}
        {message.interest && (
          <div className="admin-form-group">
            <span className="admin-form-label">Interest</span>
            <p>{message.interest}</p>
          </div>
        )}
        <div className="admin-form-group">
          <span className="admin-form-label">Message</span>
          <p style={{ whiteSpace: "pre-wrap", lineHeight: 1.6 }}>{message.body}</p>
        </div>

        <div className="admin-form-actions">
          <a href={`mailto:${message.email}`} className="admin-btn admin-btn--ghost">Reply by email</a>
          {!message.isRead && (
            <button type="button" className="admin-btn admin-btn--primary" onClick={markRead}>Mark as read</button>
          )}
          <button type="button" className="admin-btn admin-btn--danger" onClick={remove}>Delete</button>
        </div>
      </div>
    </>
  );
}
