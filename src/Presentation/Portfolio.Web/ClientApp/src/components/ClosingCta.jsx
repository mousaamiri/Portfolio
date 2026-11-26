import { Link } from "react-router-dom";
import { useI18n } from "../lib/i18n";
import { FEATURES, ROUTES } from "../lib/features";

// Shared "Closing CTA" block used on About and Experience (mirrors _ClosingCta.cshtml).
export default function ClosingCta() {
  const { t } = useI18n();

  return (
    <div className="closing-cta-block">
      <div className="closing-cta-eyebrow-row">
        <span className="eyebrow">{t("cta.inner_mechanics", "INNER MECHANICS")}</span>
      </div>
      <h2
        className="closing-cta-heading"
        dangerouslySetInnerHTML={{
          __html: t("cta.heading", 'Where logic meets<br />\n        <span class="script-gradient">rhythm.</span>'),
        }}
      />

      <div className="closing-cta-grid">
        <div className="closing-cta-card">
          <span className="eyebrow eyebrow--sm">{t("cta.projects", "PROJECTS")}</span>
          <h4 className="closing-cta-card-title">{t("cta.resumeiq", "Vitastic")}</h4>
          <p className="closing-cta-card-desc">{t("cta.resumeiq_desc", "Online Learning Platform (DDD / .NET)")}</p>
          <Link to={ROUTES.work} className="closing-cta-link">
            <span>{t("cta.view_work", "View Work")}</span> <span className="closing-cta-link-arrow">→</span>
          </Link>
        </div>

        {FEATURES.blogEnabled && (
          <div className="closing-cta-card">
            <span className="eyebrow eyebrow--sm">{t("cta.writing", "WRITING")}</span>
            <h4 className="closing-cta-card-title">
              {t("cta.article_title", "Clean Architecture in ASP.NET Core: Boundaries that Scale")}
            </h4>
            <span className="closing-cta-meta">{t("cta.published_date", "Draft")}</span>
            <Link to={ROUTES.blog} className="closing-cta-link">
              <span>{t("cta.read_article", "Read Article")}</span> <span className="closing-cta-link-arrow">→</span>
            </Link>
          </div>
        )}

        <div className="closing-cta-card">
          <span className="eyebrow eyebrow--sm">{t("cta.network", "NETWORK")}</span>
          <h4
            className="closing-cta-card-title closing-cta-card-title--large"
            dangerouslySetInnerHTML={{
              __html: t("cta.network_title", 'Let\'s build<br /><span class="script-gradient">together</span>'),
            }}
          />
          <p className="closing-cta-card-desc">{t("cta.collab_desc", "Reach out for collaborations or just to say hi.")}</p>
          <Link to={ROUTES.contact} className="closing-cta-link">
            <span>{t("cta.connect", "Connect")}</span> <span className="closing-cta-link-arrow">→</span>
          </Link>
        </div>
      </div>
    </div>
  );
}
