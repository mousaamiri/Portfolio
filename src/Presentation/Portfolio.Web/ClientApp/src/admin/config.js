// Config-driven admin CRUD. Each entry describes one sidebar resource: its API
// path, list columns, form fields (base + per-language) and payload mapping.
// The DTOs returned by Portfolio.API are flattened per requested language, so
// editing fetches the "en" and "fa" variants separately and merges them.

const text = (name, label, extra = {}) => ({ name, label, type: "text", ...extra });
const area = (name, label, extra = {}) => ({ name, label, type: "textarea", ...extra });
const num = (name, label, extra = {}) => ({ name, label, type: "number", ...extra });
const date = (name, label, extra = {}) => ({ name, label, type: "date", ...extra });
const check = (name, label, extra = {}) => ({ name, label, type: "checkbox", ...extra });

export const RESOURCES = {
  projects: {
    resource: "projects",
    title: "Projects",
    subtitle: "Manage portfolio projects",
    createLabel: "New Project",
    columns: [
      { label: "Title", value: (d) => d.title },
      { label: "Slug", value: (d) => d.slug },
      { label: "Order", value: (d) => d.displayOrder },
    ],
    status: (d) => d.isPublished,
    statusKind: "published",
    defaults: { displayOrder: 0, isPublished: true, isFeatured: false, isClientProject: false, isSourcePrivate: false, isActive: true },
    base: [
      text("slug", "Slug", { required: true, hint: "Lowercase, hyphen-separated. Used in the public URL." }),
      num("displayOrder", "Display order"),
      text("thumbnailUrl", "Thumbnail URL", { required: true, placeholder: "/images/projects/slug.jpg" }),
      text("coverImageUrl", "Cover image URL"),
      text("previewUrl", "Preview URL", { placeholder: "https://..." }),
      text("sourceCodeUrl", "Source code URL", { placeholder: "https://github.com/..." }),
      date("startedAt", "Started at", { required: true }),
      date("completedAt", "Completed at"),
      check("isPublished", "Published"),
      check("isFeatured", "Featured"),
      check("isClientProject", "Client project"),
      check("isSourcePrivate", "Source private"),
      check("isActive", "Active"),
    ],
    translation: [
      text("title", "Title", { required: true, placeholder: "Project name" }),
      text("shortDescription", "Short description", { placeholder: "One-line description" }),
      area("description", "Description", { rows: 4 }),
      text("technologies", "Technologies", { hint: "Comma-separated." }),
      text("metaTitle", "Meta title"),
      text("metaDescription", "Meta description"),
    ],
  },

  articles: {
    resource: "articles",
    title: "Articles",
    subtitle: "Manage blog articles",
    createLabel: "New Article",
    columns: [
      { label: "Title", value: (d) => d.title },
      { label: "Category", value: (d) => d.category },
      { label: "Published", value: (d) => (d.publishDate || "").slice(0, 10) },
    ],
    status: (d) => d.isPublished,
    statusKind: "published",
    defaults: { displayOrder: 0, readTimeMinutes: 5, isPublished: true, isActive: true },
    base: [
      text("slug", "Slug", { required: true }),
      text("category", "Category"),
      text("tags", "Tags", { hint: "Comma-separated." }),
      text("coverImageUrl", "Cover image URL"),
      date("publishDate", "Publish date", { required: true }),
      num("readTimeMinutes", "Read time (minutes)"),
      num("displayOrder", "Display order"),
      check("isPublished", "Published"),
      check("isActive", "Active"),
    ],
    translation: [
      text("title", "Title", { required: true }),
      area("excerpt", "Excerpt", { rows: 3 }),
      area("body", "Body", { rows: 8 }),
    ],
  },

  experiences: {
    resource: "experiences",
    title: "Experience",
    subtitle: "Manage professional history",
    createLabel: "New Experience",
    columns: [
      { label: "Job title", value: (d) => d.jobTitle },
      { label: "Company", value: (d) => d.companyName },
      { label: "Dates", value: (d) => `${(d.startDate || "").slice(0, 10)} → ${d.endDate ? d.endDate.slice(0, 10) : "Present"}` },
    ],
    status: (d) => d.isActive,
    statusKind: "active",
    defaults: { isActive: true },
    base: [
      text("companyLogo", "Company logo URL"),
      text("companyUrl", "Company URL"),
      date("startDate", "Start date", { required: true }),
      date("endDate", "End date"),
      check("isActive", "Active"),
    ],
    translation: [
      text("companyName", "Company name", { required: true }),
      text("jobTitle", "Job title", { required: true }),
      area("description", "Description", { rows: 4 }),
      text("location", "Location"),
    ],
  },

  education: {
    resource: "educations",
    title: "Education",
    subtitle: "Manage education history",
    createLabel: "New Education",
    columns: [
      { label: "Degree", value: (d) => d.degree },
      { label: "Institution", value: (d) => d.institutionName },
      { label: "Dates", value: (d) => `${(d.startDate || "").slice(0, 10)} → ${d.endDate ? d.endDate.slice(0, 10) : "Present"}` },
    ],
    status: (d) => d.isActive,
    statusKind: "active",
    defaults: { isActive: true },
    base: [
      text("institutionLogo", "Institution logo URL"),
      text("institutionUrl", "Institution URL"),
      date("startDate", "Start date", { required: true }),
      date("endDate", "End date"),
      num("gpa", "GPA"),
      check("isActive", "Active"),
    ],
    translation: [
      text("institutionName", "Institution name", { required: true }),
      text("degree", "Degree", { required: true }),
      text("fieldOfStudy", "Field of study", { required: true }),
      area("description", "Description", { rows: 3 }),
    ],
  },

  metrics: {
    resource: "impact-metrics",
    title: "Impact Metrics",
    subtitle: "Experience-page metrics",
    createLabel: "New Metric",
    columns: [
      { label: "Value", value: (d) => d.value },
      { label: "Tag", value: (d) => d.tag },
      { label: "Color", value: (d) => d.color },
    ],
    status: (d) => d.isActive,
    statusKind: "active",
    defaults: { color: "amber", displayOrder: 0, isActive: true },
    base: [
      text("value", "Value", { required: true }),
      text("color", "Color"),
      num("displayOrder", "Display order"),
      check("isActive", "Active"),
    ],
    translation: [
      text("tag", "Tag", { required: true }),
      text("description", "Description", { required: true }),
    ],
  },

  principles: {
    resource: "principles",
    title: "Principles",
    subtitle: "Experience-page core principles",
    createLabel: "New Principle",
    columns: [
      { label: "Title", value: (d) => d.title },
      { label: "Description", value: (d) => d.description },
    ],
    status: (d) => d.isActive,
    statusKind: "active",
    defaults: { displayOrder: 0, isActive: true },
    base: [
      num("displayOrder", "Display order"),
      check("isActive", "Active"),
    ],
    translation: [
      text("title", "Title", { required: true }),
      area("description", "Description", { required: true, rows: 3 }),
    ],
  },

  proficiencies: {
    resource: "proficiencies",
    title: "Proficiency Groups",
    subtitle: "Experience-page proficiency matrix",
    createLabel: "New Group",
    columns: [
      { label: "Title", value: (d) => d.title },
      { label: "Items", value: (d) => d.items },
      { label: "Color", value: (d) => d.color },
    ],
    status: (d) => d.isActive,
    statusKind: "active",
    defaults: { color: "amber", displayOrder: 0, isActive: true },
    base: [
      text("color", "Color"),
      num("displayOrder", "Display order"),
      check("isActive", "Active"),
    ],
    translation: [
      text("title", "Title", { required: true }),
      text("items", "Items", { required: true, hint: "Comma-separated." }),
    ],
  },

  skills: {
    resource: "skills",
    title: "Skills",
    subtitle: "Manage your technical skills",
    createLabel: "New Skill",
    columns: [
      { label: "Name", value: (d) => d.name },
      { label: "Category", value: (d) => d.category },
      { label: "Proficiency", value: (d) => d.proficiency },
    ],
    status: (d) => d.isActive,
    statusKind: "active",
    defaults: { proficiency: 80, displayOrder: 0, isActive: true },
    base: [
      text("iconUrl", "Icon URL", { required: true }),
      num("proficiency", "Proficiency (0-100)"),
      num("displayOrder", "Display order"),
      check("isActive", "Active"),
    ],
    translation: [
      text("name", "Name", { required: true }),
      text("description", "Description"),
      text("category", "Category"),
    ],
  },

  timeline: {
    resource: "timeline",
    title: "Timeline",
    subtitle: "Manage About-page journey",
    createLabel: "New Entry",
    columns: [
      { label: "Year", value: (d) => d.year },
      { label: "Title", value: (d) => d.title },
      { label: "Icon", value: (d) => d.icon },
    ],
    status: (d) => d.isActive,
    statusKind: "active",
    defaults: { displayOrder: 0, isActive: true },
    base: [
      text("year", "Year", { required: true }),
      text("icon", "Icon", { required: true }),
      num("displayOrder", "Display order"),
      check("isActive", "Active"),
    ],
    translation: [
      text("title", "Title", { required: true }),
      area("description", "Description", { rows: 3 }),
    ],
  },

  interests: {
    resource: "interests",
    title: "Interests",
    subtitle: "Manage About-page interests",
    createLabel: "New Interest",
    columns: [
      { label: "Label", value: (d) => d.label },
      { label: "Icon", value: (d) => d.icon },
    ],
    status: (d) => d.isActive,
    statusKind: "active",
    defaults: { displayOrder: 0, isActive: true },
    base: [
      text("icon", "Icon", { required: true }),
      num("displayOrder", "Display order"),
      check("isActive", "Active"),
    ],
    translation: [text("label", "Label", { required: true })],
  },

  stats: {
    resource: "stats",
    title: "Stats",
    subtitle: "Manage About-page footprint counters",
    createLabel: "New Stat",
    columns: [
      { label: "Label", value: (d) => d.label },
      { label: "Target", value: (d) => d.countTarget },
      { label: "Icon", value: (d) => d.icon },
    ],
    status: (d) => d.isActive,
    statusKind: "active",
    defaults: { countTarget: 0, displayOrder: 0, isActive: true },
    base: [
      text("icon", "Icon", { required: true }),
      num("countTarget", "Count target"),
      text("suffix", "Suffix"),
      num("displayOrder", "Display order"),
      check("isActive", "Active"),
    ],
    translation: [
      text("label", "Label", { required: true }),
      text("tipText", "Tooltip text"),
      text("tipAriaLabel", "Tooltip aria label"),
    ],
  },

  faqs: {
    resource: "faqs",
    title: "FAQ",
    subtitle: "Manage contact-page questions",
    createLabel: "New Question",
    columns: [
      { label: "Question", value: (d) => d.question },
      { label: "Answer", value: (d) => d.answer },
    ],
    status: (d) => d.isActive,
    statusKind: "active",
    defaults: { displayOrder: 0, isActive: true },
    base: [
      num("displayOrder", "Display order"),
      check("isActive", "Active"),
    ],
    translation: [
      text("question", "Question", { required: true }),
      area("answer", "Answer", { required: true, rows: 3 }),
    ],
  },

  testimonials: {
    resource: "testimonials",
    title: "Testimonials",
    subtitle: "Manage endorsements",
    createLabel: "New Testimonial",
    columns: [
      { label: "Name", value: (d) => d.name },
      { label: "Role", value: (d) => d.role },
      { label: "Quote", value: (d) => d.quote },
    ],
    status: (d) => d.isActive,
    statusKind: "active",
    defaults: { avatarColor: "var(--accent)", displayOrder: 0, isActive: true },
    base: [
      text("initials", "Initials", { required: true }),
      text("avatarColor", "Avatar color"),
      num("displayOrder", "Display order"),
      check("isActive", "Active"),
    ],
    translation: [
      area("quote", "Quote", { required: true, rows: 3 }),
      text("name", "Name", { required: true }),
      text("role", "Role", { required: true }),
    ],
  },

  uitranslations: {
    resource: "ui-translations",
    listLang: "fa",
    flat: true,
    title: "UI Text",
    subtitle: "Localized interface strings (nav, labels, buttons)",
    createLabel: "New String",
    columns: [
      { label: "Key", value: (d) => d.key },
      { label: "Lang", value: (d) => d.language },
      { label: "Value", value: (d) => d.value },
    ],
    status: (d) => d.isActive,
    statusKind: "active",
    defaults: { languageCode: "fa", isActive: true },
    flatFields: [
      text("key", "Key", { required: true }),
      { name: "languageCode", label: "Language", type: "select", options: [
        { value: "fa", label: "Persian (fa)" },
        { value: "en", label: "English (en)" },
        { value: "ar", label: "Arabic (ar)" },
      ] },
      area("value", "Value", { required: true, rows: 3 }),
      check("isActive", "Active"),
    ],
  },
};

export const SIDEBAR_NAV = [
  { page: "index", label: "Dashboard", icon: "layout-dashboard", to: "/Admin" },
  { page: "profile", label: "Profile", icon: "user-circle", to: "/Admin/Profile" },
  { page: "projects", label: "Projects", icon: "folder-kanban", to: "/Admin/projects" },
  { page: "articles", label: "Articles", icon: "file-text", to: "/Admin/articles" },
  { page: "experiences", label: "Experiences", icon: "briefcase", to: "/Admin/experiences" },
  { page: "education", label: "Education", icon: "graduation-cap", to: "/Admin/education" },
  { page: "metrics", label: "Impact Metrics", icon: "trending-up", to: "/Admin/metrics" },
  { page: "principles", label: "Principles", icon: "compass", to: "/Admin/principles" },
  { page: "proficiencies", label: "Proficiencies", icon: "layers", to: "/Admin/proficiencies" },
  { page: "skills", label: "Skills", icon: "star", to: "/Admin/skills" },
  { page: "timeline", label: "Timeline", icon: "milestone", to: "/Admin/timeline" },
  { page: "interests", label: "Interests", icon: "heart", to: "/Admin/interests" },
  { page: "stats", label: "Stats", icon: "bar-chart-3", to: "/Admin/stats" },
  { page: "faqs", label: "FAQ", icon: "help-circle", to: "/Admin/faqs" },
  { page: "testimonials", label: "Testimonials", icon: "quote", to: "/Admin/testimonials" },
  { page: "uitranslations", label: "UI Text", icon: "languages", to: "/Admin/uitranslations" },
  { page: "messages", label: "Messages", icon: "mail", to: "/Admin/Messages" },
];

function coerce(field, value) {
  if (field.type === "number") {
    if (value === "" || value === null || value === undefined) return 0;
    const n = Number(value);
    return Number.isFinite(n) ? n : 0;
  }
  if (field.type === "checkbox") return !!value;
  if (field.type === "date") return value ? new Date(value).toISOString() : null;
  return value ?? "";
}

export function buildPayload(cfg, values) {
  if (cfg.flat) {
    const payload = {};
    cfg.flatFields.forEach((f) => {
      payload[f.name] = coerce(f, values[f.name]);
    });
    return payload;
  }

  const payload = {};
  cfg.base.forEach((f) => {
    payload[f.name] = coerce(f, values[f.name]);
  });

  const en = { languageCode: "en" };
  cfg.translation.forEach((f) => {
    en[f.name] = coerce(f, values[`en_${f.name}`]);
  });
  const translations = [en];

  const hasFa = cfg.translation.some((f) => String(values[`fa_${f.name}`] ?? "").trim() !== "");
  if (hasFa) {
    const fa = { languageCode: "fa" };
    cfg.translation.forEach((f) => {
      fa[f.name] = coerce(f, values[`fa_${f.name}`]);
    });
    translations.push(fa);
  }

  payload.translations = translations;
  return payload;
}

function toInputValue(field, value) {
  if (value === null || value === undefined) return field.type === "checkbox" ? false : "";
  if (field.type === "date") return String(value).slice(0, 10);
  return value;
}

export function initialValues(cfg, enDto, faDto) {
  const values = { ...cfg.defaults };

  if (cfg.flat) {
    const dto = enDto;
    if (dto) {
      values.key = dto.key ?? "";
      values.languageCode = dto.language ?? "fa";
      values.value = dto.value ?? "";
      values.isActive = dto.isActive ?? true;
    }
    return values;
  }

  cfg.base.forEach((f) => {
    if (enDto && enDto[f.name] !== undefined) values[f.name] = toInputValue(f, enDto[f.name]);
  });
  cfg.translation.forEach((f) => {
    values[`en_${f.name}`] = toInputValue(f, enDto?.[f.name]);
    values[`fa_${f.name}`] = toInputValue(f, faDto?.[f.name]);
  });
  return values;
}
