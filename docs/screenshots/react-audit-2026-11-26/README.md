# React front-end visual audit — 2026-11-26

Captured from the built SPA served by `Portfolio.Web` (`http://localhost:5266`)
against the local `Portfolio.API` (SQL Server `PortfolioDB`), 680×759 browser pane,
full-page JPEG. English screenshots use the default language; `*-fa.jpg` shots were
taken after switching the site to Persian (RTL).

| # | File | Route | Result |
|---|------|-------|--------|
| 01 | `01-home.jpg` | `/` | Hero renders: name, role, bio, actions, socials, learning card |
| 02 | `02-about.jpg` | `/About` | Bio, timeline, footprint, skills, hands-on, interests, education, endorsements |
| 03 | `03-experience.jpg` | `/Experience` | Metrics, principles, summary, job list, proficiency groups |
| 04 | `04-work.jpg` | `/Work` | Project list + preview + closing CTA |
| 05 | `05-blog.jpg` | `/Blog` | Filters, search, 3 article cards |
| 06 | `06-contact.jpg` | `/Contact` | Email/network/phone cards, form, FAQ, map, CTA |
| 07 | `07-login.jpg` | `/Login` | Customer login screen |
| 08 | `08-forgot-password.jpg` | `/ForgotPassword` | Password reset screen |
| 09 | `09-admin-login.jpg` | `/Admin/Login` | Admin sign-in screen |
| 11 | `11-admin-dashboard.jpg` | `/Admin` | Stat cards + unread messages |
| 12 | `12-admin-projects.jpg` | `/Admin/projects` | Projects list (3 rows) |
| 13 | `13-admin-articles.jpg` | `/Admin/articles` | Articles list (3 rows) |
| 14 | `14-admin-messages.jpg` | `/Admin/Messages` | Inbox (2 messages, 1 unread) |
| 15 | `15-admin-profile.jpg` | `/Admin/Profile` | Bilingual profile form |
| 16 | `16-admin-project-new.jpg` | `/Admin/projects/new` | Create-project form |
| 17 | `17-admin-skills.jpg` | `/Admin/skills` | Skills list (19 rows) |
| 18 | `18-admin-ui-text.jpg` | `/Admin/uitranslations` | UI translation list |
| 10 | `10-home-fa.jpg` | `/` (fa) | Persian hero, RTL layout |
| 19 | `19-about-fa.jpg` | `/About` (fa) | Persian about page |
| 20 | `20-experience-fa.jpg` | `/Experience` (fa) | Persian experience page |
| 21 | `21-work-fa.jpg` | `/Work` (fa) | Persian work page |
| 22 | `22-api-down-state.jpg` | `/About` with API stopped | `PageStatus` error + Retry instead of a blank section |

## Notes

- The admin panel is fully built and reachable; login uses the dev seed
  `admin` / `Admin@123456` and stores the JWT in `localStorage`.
- The SPA now calls `/api/*` on its own origin; `Portfolio.Web` proxies those calls
  to `Portfolio.API`, so browser CORS and the self-signed dev certificate no longer
  blank the pages.
- Unknown paths are served `index.html`, so the React 404 page is reachable through
  the .NET host as well.
- Fixed: the Work heading rendered `SELECTED SYSTEMS &amp; ARCHITECTURE` literally
  (HTML entities in UI-translation values are now decoded once in `t()`).
- Fixed: the mobile navbar lost the theme and language switches; they are back in
  the mobile panel footer.
- The admin sidebar collapses into an off-canvas drawer below ~900px; use the
  hamburger in the admin header to reach navigation and logout.
