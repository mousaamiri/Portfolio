import { useEffect, useState } from "react";
import { Link, Outlet, useLocation, useNavigate } from "react-router-dom";
import "../styles/admin/admin.css";
import { Icon } from "../lib/icons";
import { useTheme } from "../lib/theme";
import { clearToken } from "../lib/api";
import { RESOURCES, SIDEBAR_NAV } from "./config";
import { ToastProvider } from "./toast";

function headerTitle(pathname) {
  const rest = pathname.split("/").filter(Boolean).slice(1);
  if (rest.length === 0) return "Dashboard";

  const key = rest[0];
  if (key === "Profile") return "Profile";
  if (key === "ChangePassword") return "Change Password";
  if (key === "Messages") return rest[1] ? "Message" : "Messages";

  const cfg = RESOURCES[key.toLowerCase()];
  if (!cfg) return "Admin";
  const singular = cfg.title.replace(/s$/, "");
  if (rest[1] === "new") return `New ${singular}`;
  if (rest[2] === "edit") return `Edit ${singular}`;
  return cfg.title;
}

function activePage(pathname) {
  if (pathname === "/Admin" || pathname === "/Admin/") return "index";
  const seg = pathname.split("/").filter(Boolean)[1] || "";
  if (seg === "Profile") return "profile";
  if (seg === "Messages") return "messages";
  return seg.toLowerCase();
}

export default function AdminLayout() {
  const { theme, toggleTheme } = useTheme();
  const location = useLocation();
  const navigate = useNavigate();
  const [mobileOpen, setMobileOpen] = useState(false);

  // Admin panel is English/LTR regardless of the public site's language.
  useEffect(() => {
    const prevDir = document.documentElement.getAttribute("dir");
    const prevLang = document.documentElement.getAttribute("lang");
    document.documentElement.setAttribute("dir", "ltr");
    document.documentElement.setAttribute("lang", "en");
    return () => {
      if (prevDir) document.documentElement.setAttribute("dir", prevDir);
      if (prevLang) document.documentElement.setAttribute("lang", prevLang);
    };
  }, []);

  useEffect(() => {
    setMobileOpen(false);
  }, [location.pathname]);

  const current = activePage(location.pathname);
  const logoSrc = `/images/logo-en-${theme === "light" ? "light" : "dark"}.svg`;

  const logout = () => {
    clearToken();
    navigate("/Admin/Login", { replace: true });
  };

  return (
    <ToastProvider>
      <aside className={`admin-sidebar${mobileOpen ? " is-open" : ""}`}>
        <div className="admin-sidebar-header">
          <img src={logoSrc} alt="Logo" className="admin-sidebar-logo" />
          <span className="admin-sidebar-brand">Admin</span>
        </div>
        <nav className="admin-nav">
          {SIDEBAR_NAV.map((item) => (
            <Link
              key={item.page}
              to={item.to}
              className={`admin-nav-link${current === item.page ? " is-active" : ""}`}
            >
              <Icon name={item.icon} className="admin-nav-icon" size={18} />
              <span>{item.label}</span>
            </Link>
          ))}
        </nav>
        <div className="admin-sidebar-footer">
          <Link to="/Admin/ChangePassword" className="admin-nav-link admin-nav-link--subtle">
            <Icon name="key-round" className="admin-nav-icon" size={18} />
            <span>Change Password</span>
          </Link>
          <a href="/" className="admin-nav-link admin-nav-link--subtle" target="_blank" rel="noopener noreferrer">
            <Icon name="external-link" className="admin-nav-icon" size={18} />
            <span>View Site</span>
          </a>
          <button type="button" className="admin-nav-link admin-nav-link--subtle" onClick={toggleTheme}>
            <Icon name={theme === "dark" ? "sun" : "moon"} className="admin-nav-icon" size={18} />
            <span>{theme === "dark" ? "Light Mode" : "Dark Mode"}</span>
          </button>
          <button type="button" className="admin-nav-link admin-nav-link--subtle" onClick={logout}>
            <Icon name="log-out" className="admin-nav-icon" size={18} />
            <span>Logout</span>
          </button>
        </div>
      </aside>

      <div
        className={`admin-sidebar-overlay${mobileOpen ? " is-visible" : ""}`}
        onClick={() => setMobileOpen(false)}
      ></div>

      <div className="admin-main">
        <header className="admin-header">
          <button className="admin-hamburger" type="button" aria-label="Toggle sidebar" onClick={() => setMobileOpen((v) => !v)}>
            <Icon name="menu" size={20} />
          </button>
          <span className="admin-header-title">{headerTitle(location.pathname)}</span>
          <div className="admin-header-right"></div>
        </header>
        <main className="admin-content">
          <Outlet />
        </main>
      </div>
    </ToastProvider>
  );
}
