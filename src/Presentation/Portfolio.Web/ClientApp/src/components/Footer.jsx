import { Link } from "react-router-dom";
import { Icon } from "../lib/icons";
import { useI18n } from "../lib/i18n";
import { FEATURES, ROUTES } from "../lib/features";

export default function Footer() {
  const { t } = useI18n();

  return (
    <footer className="site-footer">
      <div className="footer-inner">
        <div className="footer-top">
          <div className="footer-brand">
            <Link to={ROUTES.home} className="footer-logo">
              mousa <span>.dev</span>
            </Link>
            <span className="footer-credit">
              {t("footer.developed_with", "Developed with ❤ by")}{" "}
              <Link to={ROUTES.about}>Mousa Amiri Motlagh</Link>
            </span>
          </div>

          <nav className="footer-nav" aria-label="Footer navigation">
            <Link to={ROUTES.home} className="footer-nav-link">{t("footer.home", "Home")}</Link>
            <Link to={ROUTES.about} className="footer-nav-link">{t("footer.about", "About")}</Link>
            <Link to={ROUTES.experience} className="footer-nav-link">{t("footer.experience", "Experience")}</Link>
            <Link to={ROUTES.work} className="footer-nav-link">{t("footer.work", "Work")}</Link>
            {FEATURES.blogEnabled && (
              <Link to={ROUTES.blog} className="footer-nav-link">{t("footer.blog", "Blog")}</Link>
            )}
            <Link to={ROUTES.contact} className="footer-nav-link">{t("footer.contact", "Contact")}</Link>
          </nav>

          <div className="footer-social">
            <a href="#" className="footer-social-link" aria-label="LinkedIn"><Icon name="linkedin" size={18} /></a>
            <a href="#" className="footer-social-link" aria-label="Instagram"><Icon name="instagram" size={18} /></a>
            <a href="#" className="footer-social-link" aria-label="Telegram"><Icon name="send" size={18} /></a>
            <a href="https://github.com/mousaamiri/" className="footer-social-link" aria-label="GitHub" target="_blank" rel="noopener noreferrer"><Icon name="github" size={18} /></a>
            <a href="#" className="footer-social-link" aria-label="X (Twitter)"><Icon name="twitter" size={18} /></a>
          </div>
        </div>

        <div className="footer-bottom">
          <span>{t("footer.copyright", "© 2026 Mousa Amiri Motlagh")}</span>
          <span className="footer-bottom-sep">·</span>
          <a href="#">{t("footer.privacy", "Privacy")}</a>
          <span className="footer-bottom-sep">·</span>
          <a href="#">{t("footer.terms", "Terms")}</a>
          <span className="footer-bottom-sep">·</span>
          <span className="footer-live-dot">{t("footer.viewing_now", "1 viewing now")}</span>
        </div>
      </div>
    </footer>
  );
}
