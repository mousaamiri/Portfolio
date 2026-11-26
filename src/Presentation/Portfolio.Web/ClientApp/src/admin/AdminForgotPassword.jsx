import { useEffect } from "react";
import { Link } from "react-router-dom";
import "../styles/pages/forgot-password.css";
import { usePageMeta } from "../lib/hooks";

export default function AdminForgotPassword() {
  usePageMeta("Admin — Password recovery");

  useEffect(() => {
    const prevDir = document.documentElement.getAttribute("dir");
    document.documentElement.setAttribute("dir", "ltr");
    return () => {
      if (prevDir) document.documentElement.setAttribute("dir", prevDir);
    };
  }, []);

  return (
    <section className="forgot-section">
      <div className="forgot-bg-gradient" aria-hidden="true"></div>

      <div className="forgot-card">
        <Link to="/Admin/Login" className="forgot-back-link">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="18" height="18">
            <path d="m15 18-6-6 6-6" />
          </svg>
          <span>Back to sign in</span>
        </Link>

        <h1 className="forgot-title">Password recovery</h1>
        <p className="forgot-subtitle" style={{ lineHeight: 1.7 }}>
          This portfolio has a single administrator and no email-based reset flow. To change your password, sign in and use
          <strong> Change Password</strong> from the sidebar.
        </p>
        <p className="forgot-subtitle" style={{ lineHeight: 1.7, marginTop: 14 }}>
          If you're locked out entirely, the password can be reset from the server by updating the{" "}
          <code>AdminSeed:Password</code> user-secret and reseeding, or by updating the admin record directly in the database.
        </p>
      </div>
    </section>
  );
}
