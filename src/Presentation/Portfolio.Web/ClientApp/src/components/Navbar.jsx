import { useEffect, useRef, useState } from "react";
import { Link, useLocation } from "react-router-dom";
import { Icon } from "../lib/icons";
import { useI18n } from "../lib/i18n";
import { useTheme } from "../lib/theme";
import { FEATURES, ROUTES, pageFromPath } from "../lib/features";
import Logo from "./Logo";

const LANG_ORDER = ["en", "fa"];
const LANG_SHORT = { en: "EN", fa: "فا" };
const LANG_NAME = { en: "English", fa: "فارسی" };

const NAV_ITEMS = [
  { page: "home", key: "nav.home", label: "Home" },
  { page: "about", key: "nav.about", label: "About" },
  { page: "experience", key: "nav.experience", label: "Experience" },
  { page: "work", key: "nav.work", label: "Work" },
  { page: "blog", key: "nav.blog", label: "Blog", blogOnly: true },
];

export default function Navbar({ onOpenPalette }) {
  const { lang, setLang, t } = useI18n();
  const { isDark, toggleTheme } = useTheme();
  const location = useLocation();

  const [scrolled, setScrolled] = useState(false);
  const [mobileOpen, setMobileOpen] = useState(false);
  const navRef = useRef(null);
  const pillRef = useRef(null);
  const linkRefs = useRef({});

  const activePage = pageFromPath(location.pathname);
  const items = NAV_ITEMS.filter((i) => !i.blogOnly || FEATURES.blogEnabled);

  useEffect(() => {
    const onScroll = () => setScrolled(window.scrollY > 50);
    onScroll();
    window.addEventListener("scroll", onScroll, { passive: true });
    return () => window.removeEventListener("scroll", onScroll);
  }, []);

  useEffect(() => {
    setMobileOpen(false);
  }, [location.pathname]);

  useEffect(() => {
    document.body.style.overflow = mobileOpen ? "hidden" : "";
    return () => {
      document.body.style.overflow = "";
    };
  }, [mobileOpen]);

  // Move the sliding active pill under the active link.
  useEffect(() => {
    const move = () => {
      const link = linkRefs.current[activePage];
      const pill = pillRef.current;
      const nav = navRef.current;
      if (!link || !pill || !nav) {
        if (pill) pill.classList.remove("visible");
        return;
      }
      const navRect = nav.getBoundingClientRect();
      const linkRect = link.getBoundingClientRect();
      pill.style.left = `${linkRect.left - navRect.left}px`;
      pill.style.width = `${linkRect.width}px`;
      pill.classList.add("visible");
    };
    const raf = requestAnimationFrame(move);
    window.addEventListener("resize", move);
    return () => {
      cancelAnimationFrame(raf);
      window.removeEventListener("resize", move);
    };
  }, [activePage, lang]);

  const nextLang = LANG_ORDER[(LANG_ORDER.indexOf(lang) + 1) % LANG_ORDER.length] || "fa";

  return (
    <header className={`navbar${scrolled ? " scrolled" : ""}`} id="navbar">
      <div className="navbar-inner">
        <Link to={ROUTES.home} className="navbar-logo" aria-label="Home">
          <Logo lang={lang} />
        </Link>

        <div className="navbar-pill">
          <nav className="navbar-nav" aria-label="Main navigation" ref={navRef}>
            <div className="navbar-active-pill" aria-hidden="true" ref={pillRef}></div>
            <ul className="navbar-links" role="list">
              {items.map((item) => (
                <li key={item.page}>
                  <Link
                    to={ROUTES[item.page]}
                    className={`navbar-link${activePage === item.page ? " active" : ""}`}
                    ref={(el) => {
                      linkRefs.current[item.page] = el;
                    }}
                  >
                    {t(item.key, item.label)}
                  </Link>
                </li>
              ))}
            </ul>
          </nav>

          <div className="navbar-divider" aria-hidden="true"></div>

          <button
            className="navbar-icon-btn navbar-theme-toggle"
            type="button"
            aria-label={isDark ? "Switch to light theme" : "Switch to dark theme"}
            onClick={toggleTheme}
          >
            {isDark ? <Icon name="moon" size={18} /> : <Icon name="sun" size={18} />}
          </button>

          <button
            className="lang-switch-btn"
            type="button"
            aria-label={`Switch language to ${LANG_NAME[nextLang]}`}
            title={`Switch language to ${LANG_NAME[nextLang]}`}
            onClick={() => setLang(nextLang)}
          >
            {LANG_SHORT[nextLang]}
          </button>

          <Link
            to={ROUTES.contact}
            className={`navbar-contact-btn${activePage === "contact" ? " active" : ""}`}
          >
            {t("nav.contact", "Contact")}
          </Link>
        </div>

        <button
          className="navbar-cmd-btn"
          type="button"
          aria-label="Open command palette (Ctrl+K)"
          onClick={onOpenPalette}
        >
          <Icon name="command" size={16} />
        </button>

        <button
          className="navbar-hamburger"
          type="button"
          aria-label={t("nav.open_menu", "Open navigation menu")}
          aria-expanded={mobileOpen}
          onClick={() => setMobileOpen((v) => !v)}
        >
          <span className="navbar-hamburger-line"></span>
          <span className="navbar-hamburger-line"></span>
          <span className="navbar-hamburger-line"></span>
        </button>
      </div>

      <div
        className={`navbar-mobile-panel${mobileOpen ? " open" : ""}`}
        aria-hidden={!mobileOpen}
        style={{ display: mobileOpen ? "flex" : undefined }}
      >
        <button
          className="navbar-mobile-close"
          type="button"
          aria-label={t("nav.close_menu", "Close menu")}
          onClick={() => setMobileOpen(false)}
        >
          <Icon name="x" size={24} />
        </button>
        <nav aria-label="Mobile navigation">
          <ul className="navbar-mobile-links" role="list">
            {items.map((item) => (
              <li key={item.page}>
                <Link
                  to={ROUTES[item.page]}
                  className={`navbar-mobile-link${activePage === item.page ? " active" : ""}`}
                  onClick={() => setMobileOpen(false)}
                >
                  {t(item.key, item.label)}
                </Link>
              </li>
            ))}
            <li>
              <Link to={ROUTES.contact} className="navbar-mobile-link" onClick={() => setMobileOpen(false)}>
                {t("nav.contact", "Contact")}
              </Link>
            </li>
          </ul>
        </nav>

        <div className="navbar-mobile-footer">
          <button
            className="navbar-mobile-theme-toggle"
            type="button"
            aria-label={isDark ? "Switch to light theme" : "Switch to dark theme"}
            onClick={toggleTheme}
          >
            {isDark ? <Icon name="moon" size={18} /> : <Icon name="sun" size={18} />}
          </button>
          <button
            className="navbar-mobile-lang-switch"
            type="button"
            aria-label={`Switch language to ${LANG_NAME[nextLang]}`}
            onClick={() => setLang(nextLang)}
          >
            {LANG_NAME[nextLang]}
          </button>
          <Link
            to={ROUTES.contact}
            className={`navbar-mobile-contact${activePage === "contact" ? " active" : ""}`}
            onClick={() => setMobileOpen(false)}
          >
            {t("nav.contact", "Contact")}
          </Link>
        </div>
      </div>
    </header>
  );
}
