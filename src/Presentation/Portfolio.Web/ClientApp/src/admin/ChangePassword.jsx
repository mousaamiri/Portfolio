import { useState } from "react";
import { Link } from "react-router-dom";
import { api } from "../lib/api";
import { usePageMeta } from "../lib/hooks";
import { useToast } from "./toast";

export default function ChangePassword() {
  usePageMeta("Admin — Change Password");
  const { showToast } = useToast();

  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [saving, setSaving] = useState(false);

  const onSubmit = async (e) => {
    e.preventDefault();
    setError("");

    if (!currentPassword) return setError("Current password is required.");
    if (newPassword.length < 8) return setError("New password must be at least 8 characters.");
    if (newPassword !== confirmPassword) return setError("Passwords do not match.");

    setSaving(true);
    try {
      const ok = await api.adminChangePassword(currentPassword, newPassword);
      if (ok) {
        showToast("Password changed.");
        setCurrentPassword("");
        setNewPassword("");
        setConfirmPassword("");
      } else {
        setError("Current password is incorrect.");
      }
    } catch {
      setError("Could not change the password. Please try again.");
    } finally {
      setSaving(false);
    }
  };

  return (
    <>
      <div className="admin-page-header">
        <div>
          <h1 className="admin-page-title">Change Password</h1>
          <p className="admin-page-subtitle">Update your admin account password</p>
        </div>
        <Link to="/Admin" className="admin-btn admin-btn--ghost">Back to dashboard</Link>
      </div>

      <form className="admin-form" style={{ maxWidth: 520 }} onSubmit={onSubmit}>
        {error && <div className="admin-form-error" role="alert">{error}</div>}

        <div className="admin-form-group">
          <label className="admin-form-label" htmlFor="currentPassword">Current password</label>
          <input id="currentPassword" type="password" className="admin-form-input" autoComplete="current-password" value={currentPassword} onChange={(e) => setCurrentPassword(e.target.value)} />
        </div>
        <div className="admin-form-group">
          <label className="admin-form-label" htmlFor="newPassword">New password</label>
          <input id="newPassword" type="password" className="admin-form-input" autoComplete="new-password" value={newPassword} onChange={(e) => setNewPassword(e.target.value)} />
          <span className="admin-form-hint">At least 8 characters.</span>
        </div>
        <div className="admin-form-group">
          <label className="admin-form-label" htmlFor="confirmPassword">Confirm password</label>
          <input id="confirmPassword" type="password" className="admin-form-input" autoComplete="new-password" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} />
        </div>

        <div className="admin-form-actions">
          <Link to="/Admin" className="admin-btn admin-btn--ghost">Cancel</Link>
          <button type="submit" className="admin-btn admin-btn--primary" disabled={saving}>
            {saving ? "Saving…" : "Change password"}
          </button>
        </div>
      </form>
    </>
  );
}
