import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import "../styles/pages/work.css";
import { Icon } from "../lib/icons";
import { useI18n } from "../lib/i18n";
import { usePageMeta, useApi } from "../lib/hooks";
import { api } from "../lib/api";
import { FEATURES, ROUTES } from "../lib/features";
import { modernImageSrc, splitTechs } from "../lib/techColor";
import {
  CATEGORY_KEYS,
  isRtlLang,
  localizeDigits,
  pad2,
  projectCategory,
  projectPath,
} from "../lib/projectMeta";
import PageStatus from "../components/PageStatus";

const COPY = {
  en: {
    eyebrow: "SELECTED WORK",
    heading: "Things I've built",
    lede: "Measured design paired with software engineering, aimed at problems that actually exist.",
    filtersLabel: "Filter projects by category",
    all: "All projects",
    categories: { web: "Web", product: "Product", desktop: "Desktop" },
    viewProject: "View project",
    empty: "No projects in this category yet.",
    arrow: "→",
  },
  fa: {
    eyebrow: "نمونه‌کارهای منتخب",
    heading: "چیزهایی که ساخته‌ام",
    lede: "ترکیب طراحی سنجیده و مهندسی نرم‌افزار برای حل مسئله‌های واقعی.",
    filtersLabel: "فیلتر پروژه‌ها بر اساس دسته",
    all: "همهٔ پروژه‌ها",
    categories: { web: "وب", product: "محصول", desktop: "دسکتاپ" },
    viewProject: "مشاهدهٔ پروژه",
    empty: "هنوز پروژه‌ای در این دسته نیست.",
    arrow: "←",
  },
};

function ProjectThumb({ image }) {
  if (!image) {
    return (
      <span className="work-card-placeholder" aria-hidden="true">
        <Icon name="layout-grid" size={30} />
      </span>
    );
  }
  return (
    <picture>
      <source type="image/avif" srcSet={modernImageSrc(image, "avif")} />
      <source type="image/webp" srcSet={modernImageSrc(image, "webp")} />
      <img src={image} alt="" loading="lazy" />
    </picture>
  );
}

function WorkCard({ project, copy, lang }) {
  const number = localizeDigits(pad2(project.index + 1), lang);
  const techs = project.techs.slice(0, 3);
  const extra = project.techs.length - techs.length;

  return (
    <Link className="work-card" to={projectPath(project.slug)}>
      <span className="work-card-media">
        <ProjectThumb image={project.thumbnailUrl} />
      </span>

      <span className="work-card-body">
        <span className="work-card-meta">
          <span className="work-card-index">{number}</span>
          <span className="work-card-sep" aria-hidden="true">—</span>
          <span className="work-card-category">{copy.categories[project.category]}</span>
        </span>

        <span className="work-card-title">{project.title}</span>
        <span className="work-card-sub">{project.shortDescription}</span>

        {techs.length > 0 && (
          <span className="work-card-techs">
            {techs.map((tech) => (
              <span className="work-card-tech" key={tech.name}>
                {tech.name}
              </span>
            ))}
            {extra > 0 && <span className="work-card-tech work-card-tech--more">+{localizeDigits(extra, lang)}</span>}
          </span>
        )}

        <span className="work-card-cta">
          {copy.viewProject}
          <span className="work-card-arrow" aria-hidden="true">{copy.arrow}</span>
        </span>
      </span>
    </Link>
  );
}

export default function Work() {
  const { t, lang } = useI18n();
  const copy = isRtlLang(lang) ? COPY.fa : COPY.en;
  usePageMeta(t("work.title", copy.heading), t("work.meta_description", copy.lede));

  const { data: projects, loading, error } = useApi(() => api.getProjects(lang), [lang]);
  const [filter, setFilter] = useState("all");

  const items = useMemo(
    () =>
      (projects || []).map((project, index) => ({
        ...project,
        index,
        category: projectCategory(project),
        techs: splitTechs(project.technologies),
      })),
    [projects]
  );

  const counts = useMemo(() => {
    const map = { all: items.length };
    for (const key of CATEGORY_KEYS) map[key] = 0;
    for (const item of items) map[item.category] = (map[item.category] || 0) + 1;
    return map;
  }, [items]);

  const filters = useMemo(
    () => ["all", ...CATEGORY_KEYS.filter((key) => counts[key] > 0)],
    [counts]
  );

  // A refetch can drop the category that is currently selected; fall back to "all".
  const activeFilter = filters.includes(filter) ? filter : "all";

  const visible =
    activeFilter === "all" ? items : items.filter((item) => item.category === activeFilter);

  if (!projects) {
    return (
      <section className="work-section">
        <div className="container">
          <PageStatus loading={loading} error={error} />
        </div>
      </section>
    );
  }

  return (
    <section id="work" className="work-section">
      <div className="container">
        <header className="work-hero">
          <p className="work-eyebrow">
            <span className="work-eyebrow-index">{localizeDigits("01", lang)}</span>
            <span className="work-eyebrow-sep" aria-hidden="true">/</span>
            <span className="work-eyebrow-label">{t("work.hero_eyebrow", copy.eyebrow)}</span>
          </p>

          <h1 className="work-title">{t("work.hero_title", copy.heading)}</h1>
          <p className="work-lede">{t("work.hero_lede", copy.lede)}</p>

          <div className="work-filters" role="group" aria-label={copy.filtersLabel}>
            {filters.map((key) => (
              <button
                key={key}
                type="button"
                className={`work-filter${activeFilter === key ? " is-active" : ""}`}
                aria-pressed={activeFilter === key}
                onClick={() => setFilter(key)}
              >
                <span>{key === "all" ? copy.all : copy.categories[key]}</span>
                <span className="work-filter-count">({localizeDigits(pad2(counts[key] || 0), lang)})</span>
              </button>
            ))}
          </div>
        </header>

        {visible.length > 0 ? (
          <div className="work-grid">
            {visible.map((project) => (
              <WorkCard key={project.id || project.slug} project={project} copy={copy} lang={lang} />
            ))}
          </div>
        ) : (
          <p className="work-empty">{copy.empty}</p>
        )}
      </div>

      <div className="closing-cta-block">
        <div className="closing-cta-eyebrow-row">
          <span className="eyebrow">{t("cta.inner_mechanics", "INNER MECHANICS")}</span>
        </div>
        <h2
          className="closing-cta-heading"
          dangerouslySetInnerHTML={{
            __html: t("cta.heading", 'Where logic meets<br />\n            <span class="script-gradient">rhythm.</span>'),
          }}
        />

        <div className="closing-cta-grid">
          <div className="closing-cta-card">
            <div className="closing-cta-github-header">
              <Icon name="github" className="closing-cta-github-icon" size={22} />
              <span className="closing-cta-github-name">{t("cta.github_name", "Mousa's Github")}</span>
            </div>
            <div className="closing-cta-latest-push">
              <span className="closing-cta-latest-label">{t("cta.latest_push", "LATEST PUSH")}</span>
              <span className="closing-cta-latest-badge">{t("cta.ago", "Recently")}</span>
            </div>
            <p className="closing-cta-commit-msg">{t("cta.pushed_updates", "Latest commits")}</p>
            <div className="closing-cta-commit-files">
              <span className="closing-cta-file-badge">portfolio</span>
            </div>
            <div className="closing-cta-github-stats">
              <div>
                <span className="closing-cta-stat-number">3</span>
                <span className="closing-cta-stat-label">{t("cta.public_repos", "PUBLIC REPOS")}</span>
              </div>
              <div>
                <span className="closing-cta-stat-number">—</span>
                <span className="closing-cta-stat-label">{t("cta.followers", "FOLLOWERS")}</span>
              </div>
            </div>
            <div className="closing-cta-social-row">
              <a href="https://github.com/mousaamiri" aria-label="GitHub"><Icon name="github" className="closing-cta-social-icon" size={20} /></a>
              <a href="#" aria-label="LinkedIn"><Icon name="linkedin" className="closing-cta-social-icon" size={20} /></a>
              <a href="#" aria-label="X (Twitter)"><Icon name="twitter" className="closing-cta-social-icon" size={20} /></a>
            </div>
          </div>

          {FEATURES.blogEnabled && (
            <div className="closing-cta-card">
              <span className="eyebrow eyebrow--sm">{t("cta.writing", "WRITING")}</span>
              <h4 className="closing-cta-card-title">
                {t("cta.article_title", "Clean Architecture in ASP.NET Core: Boundaries that Scale")}
              </h4>
              <span className="closing-cta-meta">{t("cta.published_date", "Draft")}</span>
              <Link to={ROUTES.blog} className="closing-cta-link">
                <span>{t("cta.read_article", "Read Article")}</span>
                <span className="closing-cta-link-arrow">→</span>
              </Link>
            </div>
          )}

          <div className="closing-cta-card">
            <span className="eyebrow eyebrow--sm">{t("cta.workspace", "WORKSPACE")}</span>
            <h4
              className="closing-cta-card-title--large"
              dangerouslySetInnerHTML={{
                __html: t("cta.gear_title", 'My daily<br />\n                    <span class="script-gradient">drivers</span>'),
              }}
            />
            <p className="closing-cta-card-desc">{t("cta.gear_desc", "A detailed list of the software and hardware I use every day.")}</p>
            <a href="#" className="closing-cta-link">
              <span>{t("cta.view_gear", "View Gear")}</span>
              <span className="closing-cta-link-arrow">→</span>
            </a>
          </div>
        </div>
      </div>
    </section>
  );
}
