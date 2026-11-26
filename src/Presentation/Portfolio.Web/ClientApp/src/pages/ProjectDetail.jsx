import { Link, useParams } from "react-router-dom";
import "../styles/pages/project-detail.css";
import { useI18n } from "../lib/i18n";
import { useApi, usePageMeta } from "../lib/hooks";
import { api } from "../lib/api";
import { ROUTES } from "../lib/features";
import { modernImageSrc, splitTechs } from "../lib/techColor";
import {
  isRtlLang,
  localizeDigits,
  pad2,
  projectCategory,
  projectDuration,
  projectPath,
  projectYear,
  splitParagraphs,
} from "../lib/projectMeta";
import PageStatus from "../components/PageStatus";

const copy = {
  en: {
    breadcrumb: "Work",
    breadcrumbLabel: "Breadcrumb",
    eyebrow: "CASE STUDY",
    viewSite: "View website",
    github: "GitHub",
    year: "Year",
    duration: "Duration",
    role: "Role",
    roleValue: "Design & Development",
    category: "Category",
    months: "months",
    ongoing: "In progress",
    overview: "OVERVIEW",
    overviewTitle: "About the project",
    approach: "THE APPROACH",
    approachTitle: "Approach & process",
    outcome: "OUTCOME",
    outcomeTitle: "Outcome",
    techTitle: "Technologies used",
    asideGithub: "View code on GitHub",
    factsTitle: "Key facts",
    statusDone: "Completed",
    statusOngoing: "In progress",
    sourcePublic: "Public code",
    sourcePrivate: "Private code",
    techUnit: "technologies",
    allProjects: "All projects",
    prevProject: "Previous project",
    nextProject: "Next project",
    nextTitle: "Next case study",
    viewProject: "View project",
    back: "Back to all work",
    backArrow: "←",
    notFound: "Project not found",
    notFoundBack: "Back to work",
    placeholder: "No preview image",
    categories: { web: "Web", product: "Product", desktop: "Desktop" },
  },
  fa: {
    breadcrumb: "نمونه‌کارها",
    breadcrumbLabel: "مسیر صفحه",
    eyebrow: "CASE STUDY",
    viewSite: "مشاهدهٔ وب‌سایت",
    github: "گیت‌هاب",
    year: "سال",
    duration: "مدت",
    role: "نقش",
    roleValue: "طراحی و توسعه",
    category: "دسته‌بندی",
    months: "ماه",
    ongoing: "در جریان",
    overview: "OVERVIEW",
    overviewTitle: "دربارهٔ پروژه",
    approach: "THE APPROACH",
    approachTitle: "راهکار و روند",
    outcome: "OUTCOME",
    outcomeTitle: "نتیجه",
    techTitle: "فناوری‌های استفاده‌شده",
    asideGithub: "مشاهدهٔ کد در گیت‌هاب",
    factsTitle: "نکات کلیدی",
    statusDone: "تکمیل‌شده",
    statusOngoing: "در جریان",
    sourcePublic: "کد عمومی",
    sourcePrivate: "کد خصوصی",
    techUnit: "فناوری",
    allProjects: "همهٔ نمونه‌کارها",
    prevProject: "پروژهٔ قبلی",
    nextProject: "پروژهٔ بعدی",
    nextTitle: "نمونه‌کار بعدی",
    viewProject: "مشاهدهٔ پروژه",
    back: "بازگشت به فهرست نمونه‌کارها",
    backArrow: "→",
    notFound: "پروژه پیدا نشد",
    notFoundBack: "بازگشت به نمونه‌کارها",
    placeholder: "بدون تصویر پیش‌نمایش",
    categories: { web: "وب", product: "محصول", desktop: "دسکتاپ" },
  },
};

export default function ProjectDetail() {
  const { slug } = useParams();
  const { t, lang } = useI18n();
  const fa = isRtlLang(lang);
  const dict = fa ? copy.fa : copy.en;
  const c = (key) => t(`case.${key}`, dict[key]);

  const { data: projects, loading, error } = useApi(() => api.getProjects(lang), [lang]);
  const list = projects || [];
  const index = list.findIndex((item) => item.slug === slug);
  const project = index >= 0 ? list[index] : null;

  usePageMeta(
    project?.metaTitle || project?.title,
    project?.metaDescription || project?.shortDescription
  );

  let body;
  if (!projects) {
    body = <PageStatus loading={loading} error={error} />;
  } else if (!project) {
    body = (
      <div className="case-empty">
        <h1 className="case-empty-title">{c("notFound")}</h1>
        <Link className="case-empty-link" to={ROUTES.work}>
          {c("notFoundBack")}
        </Link>
      </div>
    );
  } else {
    const techs = splitTechs(project.technologies);
    const categoryKey = projectCategory(project);
    const categoryLabel = t(`case.category_${categoryKey}`, dict.categories[categoryKey]);
    const year = projectYear(project, lang);
    const duration = projectDuration(project);
    const paragraphs = splitParagraphs(project.description);
    const story = paragraphs.length
      ? paragraphs
      : project.shortDescription
        ? [project.shortDescription]
        : [];

    const sections = [];
    if (story.length) {
      sections.push({ eyebrow: c("overview"), title: c("overviewTitle"), text: story[0] });
      if (story[1]) {
        sections.push({ eyebrow: c("approach"), title: c("approachTitle"), text: story[1] });
      }
      if (story.length > 2) {
        sections.push({
          eyebrow: c("outcome"),
          title: c("outcomeTitle"),
          text: story.slice(2).join("\n\n"),
        });
      }
    }

    const durationLabel = !duration
      ? "—"
      : duration.ongoing
        ? c("ongoing")
        : `${localizeDigits(duration.months, lang)} ${c("months")}`;

    const metaCells = [
      { label: c("year"), value: year ? localizeDigits(year, lang) : "—" },
      { label: c("duration"), value: durationLabel },
      { label: c("role"), value: c("roleValue") },
      { label: c("category"), value: categoryLabel },
    ];

    const hasPreview = Boolean(project.previewUrl);
    const hasSource = Boolean(project.sourceCodeUrl) && !project.isSourcePrivate;
    const image = project.coverImageUrl || project.thumbnailUrl;
    const avif = image ? modernImageSrc(image, "avif") : "";
    const webp = image ? modernImageSrc(image, "webp") : "";

    const facts = [
      duration && !duration.ongoing ? c("statusDone") : c("statusOngoing"),
      project.isSourcePrivate ? c("sourcePrivate") : c("sourcePublic"),
      `${localizeDigits(techs.length, lang)} ${c("techUnit")}`,
    ];

    const prev = index > 0 ? list[index - 1] : null;
    const next = index < list.length - 1 ? list[index + 1] : null;
    const hasNextCard = list.length > 1;
    const nextIndex = list.length ? (index + 1) % list.length : 0;
    const nextProject = hasNextCard ? list[nextIndex] : null;
    const nextCategory = nextProject ? projectCategory(nextProject) : null;

    body = (
      <>
        <nav className="case-breadcrumb" aria-label={c("breadcrumbLabel")}>
          <Link to={ROUTES.work}>{c("breadcrumb")}</Link>
          <span className="case-breadcrumb-sep" aria-hidden="true">
            /
          </span>
          <span aria-current="page">{project.title}</span>
        </nav>

        <header className="case-header">
          <span className="case-eyebrow">
            {c("eyebrow")} — {localizeDigits(pad2(index + 1), lang)}
          </span>
          <h1 className="case-title">{project.title}</h1>
          {project.shortDescription ? (
            <p className="case-lede">{project.shortDescription}</p>
          ) : null}

          {hasPreview || hasSource ? (
            <div className="case-actions">
              {hasPreview ? (
                <a
                  className="case-btn case-btn--solid"
                  href={project.previewUrl}
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  {c("viewSite")} ↗
                </a>
              ) : null}
              {hasSource ? (
                <a
                  className="case-btn case-btn--outline"
                  href={project.sourceCodeUrl}
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  {c("github")} ↗
                </a>
              ) : null}
            </div>
          ) : null}
        </header>

        <figure className="case-hero">
          {image ? (
            <picture>
              {avif ? <source type="image/avif" srcSet={avif} /> : null}
              {webp ? <source type="image/webp" srcSet={webp} /> : null}
              <img src={image} alt="" />
            </picture>
          ) : (
            <span className="case-hero-placeholder">{c("placeholder")}</span>
          )}
        </figure>

        <div className="case-meta">
          {metaCells.map((cell) => (
            <div className="case-meta-cell" key={cell.label}>
              <span className="case-meta-label">{cell.label}</span>
              <span className="case-meta-value">{cell.value}</span>
            </div>
          ))}
        </div>

        <div className="case-body">
          <div className="case-story">
            {sections.map((section, i) => (
              <div className="case-block" key={i}>
                <span className="case-block-eyebrow">{section.eyebrow}</span>
                <h2 className="case-block-title">{section.title}</h2>
                {section.text.split(/\r?\n\s*\r?\n/).map((para, j) => (
                  <p key={j}>{para}</p>
                ))}
              </div>
            ))}
          </div>

          <aside className="case-aside">
            <div className="case-aside-card">
              <h2 className="case-aside-title">{c("techTitle")}</h2>
              <div className="case-aside-techs">
                {techs.length ? (
                  techs.map((tech, i) => (
                    <span className="case-chip" key={i}>
                      <span className="case-chip-dot" style={{ background: tech.color }}></span>
                      {tech.name}
                    </span>
                  ))
                ) : (
                  <span className="case-chip">—</span>
                )}
              </div>
              {hasSource ? (
                <>
                  <div className="case-aside-divider" aria-hidden="true"></div>
                  <a
                    className="case-aside-link"
                    href={project.sourceCodeUrl}
                    target="_blank"
                    rel="noopener noreferrer"
                  >
                    {c("asideGithub")} ↗
                  </a>
                </>
              ) : null}
            </div>
          </aside>
        </div>

        <div className="case-facts">
          <h2 className="case-facts-title">{c("factsTitle")}</h2>
          <div className="case-facts-items">
            {facts.map((fact, i) => (
              <span className="case-facts-item" key={i}>
                {i > 0 ? (
                  <span className="case-facts-sep" aria-hidden="true">
                    ·
                  </span>
                ) : null}
                {fact}
              </span>
            ))}
          </div>
        </div>

        <nav className="case-pager" aria-label={c("allProjects")}>
          {prev ? (
            <Link
              className="case-pager-link case-pager-link--prev"
              to={projectPath(prev.slug)}
              title={prev.title}
              aria-label={`${c("prevProject")}: ${prev.title}`}
            >
              <span className="case-pager-label">{c("prevProject")}</span>
              <span className="case-pager-title">{prev.title}</span>
            </Link>
          ) : (
            <span
              className="case-pager-link case-pager-link--prev case-pager-disabled"
              aria-disabled="true"
            >
              <span className="case-pager-label">{c("prevProject")}</span>
              <span className="case-pager-title">—</span>
            </span>
          )}

          <Link className="case-pager-all" to={ROUTES.work}>
            {c("allProjects")}
          </Link>

          {next ? (
            <Link
              className="case-pager-link case-pager-link--next"
              to={projectPath(next.slug)}
              title={next.title}
              aria-label={`${c("nextProject")}: ${next.title}`}
            >
              <span className="case-pager-label">{c("nextProject")}</span>
              <span className="case-pager-title">{next.title}</span>
            </Link>
          ) : (
            <span
              className="case-pager-link case-pager-link--next case-pager-disabled"
              aria-disabled="true"
            >
              <span className="case-pager-label">{c("nextProject")}</span>
              <span className="case-pager-title">—</span>
            </span>
          )}
        </nav>

        {nextProject ? (
          <div className="case-next">
            <h2 className="case-next-title">{c("nextTitle")}</h2>
            <Link className="case-next-card" to={projectPath(nextProject.slug)}>
              <span className="case-next-thumb">
                {nextProject.thumbnailUrl ? (
                    <img src={nextProject.thumbnailUrl} alt="" loading="lazy" />
                ) : null}
              </span>
              <span className="case-next-body">
                <span className="case-next-meta">
                  {localizeDigits(pad2(nextIndex + 1), lang)} ·{" "}
                  {t(`case.category_${nextCategory}`, dict.categories[nextCategory])}
                </span>
                <span className="case-next-name">{nextProject.title}</span>
                <span className="case-next-cta">{c("viewProject")}</span>
              </span>
            </Link>
          </div>
        ) : null}

        <Link className="case-back" to={ROUTES.work}>
          <span className="case-back-arrow" aria-hidden="true">{c("backArrow")}</span>
          {c("back")}
        </Link>
      </>
    );
  }

  return (
    <section className="case-section">
      <div className="container">{body}</div>
    </section>
  );
}
