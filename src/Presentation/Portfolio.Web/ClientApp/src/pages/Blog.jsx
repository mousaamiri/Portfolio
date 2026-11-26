import { useEffect, useMemo, useRef, useState } from "react";
import "../styles/pages/blog.css";
import { Icon } from "../lib/icons";
import { useI18n } from "../lib/i18n";
import { usePageMeta, useApi } from "../lib/hooks";
import { api } from "../lib/api";
import PageStatus from "../components/PageStatus";

function formatDate(value) {
  const d = new Date(value);
  if (Number.isNaN(d.getTime())) return "";
  return d.toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" });
}

export default function Blog() {
  const { lang } = useI18n();
  usePageMeta("Blog", "Thoughts, systems & engineering notes");
  const { data: articles, loading, error } = useApi(() => api.getArticles(lang), [lang]);

  const [search, setSearch] = useState("");
  const [category, setCategory] = useState("All");
  const [activePost, setActivePost] = useState(null);
  const [progress, setProgress] = useState(0);
  const bodyRef = useRef(null);

  const posts = useMemo(
    () =>
      (articles || []).map((p) => ({
        id: p.slug,
        title: p.title,
        excerpt: p.excerpt || "",
        category: p.category || "",
        date: p.publishDate,
        readTime: p.readTimeMinutes,
        content: p.body || "",
      })),
    [articles]
  );

  const categories = useMemo(() => {
    const list = ["All"];
    posts.forEach((p) => {
      if (p.category && !list.includes(p.category)) list.push(p.category);
    });
    return list;
  }, [posts]);

  const filtered = useMemo(() => {
    const q = search.toLowerCase().trim();
    return posts.filter((p) => {
      const matchCat = category === "All" || p.category === category;
      const matchSearch =
        !q || p.title.toLowerCase().includes(q) || p.excerpt.toLowerCase().includes(q);
      return matchCat && matchSearch;
    });
  }, [posts, search, category]);

  useEffect(() => {
    document.body.style.overflow = activePost ? "hidden" : "";
    setProgress(0);
    return () => {
      document.body.style.overflow = "";
    };
  }, [activePost]);

  useEffect(() => {
    const onKey = (e) => {
      if (e.key === "Escape") setActivePost(null);
    };
    document.addEventListener("keydown", onKey);
    return () => document.removeEventListener("keydown", onKey);
  }, []);

  if (!articles) {
    return (
      <section id="blog" className="blog-section">
        <div className="container">
          <PageStatus loading={loading} error={error} />
        </div>
      </section>
    );
  }

  return (
    <section id="blog" className="blog-section">
      <div className="container">
        <div className="about-eyebrow-row">
          <span className="eyebrow">BLOG</span>
          <span className="eyebrow-rule" aria-hidden="true"></span>
        </div>
        <h2 className="blog-heading">THOUGHTS, SYSTEMS &amp; ENGINEERING NOTES</h2>

        <div className="blog-search-wrap">
          <Icon name="search" className="blog-search-icon" size={18} />
          <input
            type="text"
            className="blog-search"
            placeholder="Search articles.."
            autoComplete="off"
            spellCheck="false"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>

        <div className="blog-filters" role="tablist" aria-label="Filter articles by category">
          {categories.map((cat) => (
            <button
              key={cat}
              className={`blog-filter-btn${category === cat ? " is-active" : ""}`}
              role="tab"
              aria-selected={category === cat}
              onClick={() => setCategory(cat)}
            >
              {cat}
            </button>
          ))}
        </div>

        <div className="blog-grid">
          {filtered.map((p) => (
            <article
              className="blog-card"
              key={p.id}
              tabIndex={0}
              onClick={() => setActivePost(p)}
              onKeyDown={(e) => {
                if (e.key === "Enter" || e.key === " ") setActivePost(p);
              }}
            >
              <div className="blog-card-top">
                <span className="blog-card-meta">{formatDate(p.date)}</span>
                <span className="blog-card-meta blog-card-meta--sep">·</span>
                <span className="blog-card-meta">{p.readTime} MIN READ</span>
                <span className="blog-card-tag">{p.category.toUpperCase()}</span>
              </div>
              <h3 className="blog-card-title">{p.title}</h3>
              <p className="blog-card-excerpt">{p.excerpt}</p>
              <span className="blog-card-link">
                Read <span className="blog-card-arrow">→</span>
              </span>
            </article>
          ))}
        </div>
      </div>

      {activePost && (
        <div
          className="blog-modal-overlay is-open"
          role="dialog"
          aria-modal="true"
          onClick={(e) => {
            if (e.target === e.currentTarget) setActivePost(null);
          }}
        >
          <div className="blog-modal">
            <div className="blog-modal-header">
              <div className="blog-modal-header-left">
                <span className="blog-modal-tag">{activePost.category.toUpperCase()}</span>
                <span className="blog-modal-date">
                  {formatDate(activePost.date)} · {activePost.readTime} MIN READ
                </span>
              </div>
              <button className="blog-modal-close" aria-label="Close" onClick={() => setActivePost(null)}>
                <Icon name="x" size={20} />
              </button>
            </div>
            <div className="blog-modal-progress-track">
              <div className="blog-modal-progress-bar" style={{ width: `${progress}%` }} />
            </div>
            <div
              className="blog-modal-body"
              ref={bodyRef}
              onScroll={(e) => {
                const el = e.currentTarget;
                const max = el.scrollHeight - el.clientHeight;
                setProgress(max > 0 ? Math.min(100, Math.round((el.scrollTop / max) * 100)) : 0);
              }}
            >
              <h2 className="blog-modal-title">{activePost.title}</h2>
              <div className="blog-modal-content" dangerouslySetInnerHTML={{ __html: activePost.content }} />
            </div>
          </div>
        </div>
      )}
    </section>
  );
}
