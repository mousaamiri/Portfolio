import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Icon } from "../lib/icons";
import { api } from "../lib/api";
import { usePageMeta } from "../lib/hooks";

const CARDS = [
  { key: "projects", label: "Projects", icon: "folder-kanban", fg: "#f0a63b", bg: "rgba(240,166,59,0.12)", to: "/Admin/projects" },
  { key: "articles", label: "Articles", icon: "file-text", fg: "#3b82f6", bg: "rgba(59,130,246,0.12)", to: "/Admin/articles" },
  { key: "experiences", label: "Experiences", icon: "briefcase", fg: "#22c55e", bg: "rgba(34,197,94,0.12)", to: "/Admin/experiences" },
  { key: "education", label: "Education", icon: "graduation-cap", fg: "#a855f7", bg: "rgba(168,85,247,0.12)", to: "/Admin/education" },
  { key: "skills", label: "Skills", icon: "star", fg: "#14b8a6", bg: "rgba(20,184,166,0.12)", to: "/Admin/skills" },
  { key: "testimonials", label: "Testimonials", icon: "quote", fg: "#ec4899", bg: "rgba(236,72,153,0.12)", to: "/Admin/testimonials" },
  { key: "messages", label: "Messages", icon: "mail", fg: "#ef4444", bg: "rgba(239,68,68,0.12)", to: "/Admin/Messages" },
];

export default function Dashboard() {
  usePageMeta("Admin — Dashboard");
  const [counts, setCounts] = useState({});
  const [unread, setUnread] = useState(0);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let active = true;
    Promise.all([
      api.adminList("projects"),
      api.adminList("articles"),
      api.adminList("experiences"),
      api.adminList("educations"),
      api.adminList("skills"),
      api.adminList("testimonials"),
      api.adminMessages(),
    ])
      .then(([projects, articles, experiences, education, skills, testimonials, messages]) => {
        if (!active) return;
        setCounts({
          projects: projects.length,
          articles: articles.length,
          experiences: experiences.length,
          education: education.length,
          skills: skills.length,
          testimonials: testimonials.length,
          messages: messages.length,
        });
        setUnread(messages.filter((m) => !m.isRead).length);
      })
      .catch(() => {
        /* handled globally via admin-unauthorized */
      })
      .finally(() => {
        if (active) setLoading(false);
      });
    return () => {
      active = false;
    };
  }, []);

  return (
    <>
      <div className="admin-page-header">
        <div>
          <h1 className="admin-page-title">Dashboard</h1>
          <p className="admin-page-subtitle">Overview of your portfolio content</p>
        </div>
      </div>

      <div className="admin-stats-grid">
        {CARDS.map((c) => (
          <Link key={c.key} to={c.to} className="admin-stat-card">
            <div className="admin-stat-header">
              <span className="admin-stat-label">{c.label}</span>
              <div className="admin-stat-icon" style={{ background: c.bg, color: c.fg }}>
                <Icon name={c.icon} size={20} />
              </div>
            </div>
            <div className="admin-stat-value">{loading ? "—" : counts[c.key] ?? 0}</div>
            {c.key === "messages" && !loading && <div className="admin-stat-sub">{unread} unread</div>}
          </Link>
        ))}
      </div>
    </>
  );
}
