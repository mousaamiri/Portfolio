# Portfolio Migration Package (v2 — includes admin panel)

Complete real static site + admin panel + `.claude/` rule files, updated
to reflect the admin panel added in this upload.

## ⚠️ One thing was deliberately removed from your zip

Your upload also contained a `.calude/` folder (note the typo) full of
unrelated Claude Code skills/settings (banner-design, brand, design,
slides, ui-styling, etc.) plus a `settings.local.json` and a Persian
prompt file. That looks like your own global Claude Code config folder
that got zipped up by accident — it is **not** removed from your original
zip, just excluded from this package, since it has nothing to do with the
portfolio site and shouldn't be copied into the ASP.NET project.

## Structure

```
index.html, assets/, fonts/, resumes/, shared/, pages/   — public site (6 pages + login/forgot-password showcase)
admin/                                                    — client-side-only admin CRUD panel (new)
.claude/
  PROJECT_CONTEXT.md         — includes new "Admin panel" section
  CONTENT_CLASSIFICATION.md  — includes real admin entity field shapes
  MIGRATION_PLAN.md          — includes admin panel as migration step 10 + 1 new open question
  AI_PROJECT_RULES.md        — includes admin-specific rules + the .calude warning
```

## What's new since the last package

- `/admin/` — dashboard + login + CRUD screens for Projects, Articles,
  Experiences, Education, Skills, Testimonials, Messages (contact
  submissions). Data lives in-memory only (`admin/js/data.js`), resets on
  refresh. Login is a **hardcoded, insecure demo gate**
  (`admin`/`admin` + a sessionStorage flag) — not real security.
- This is a **separate login system** from the public site's
  `pages/login/login.html` showcase page — two unrelated login screens
  now exist.
- The admin panel's real bilingual field shapes (`_en`/`_fa` suffixes)
  now give concrete ViewModel shapes for Projects/Experiences/
  Education/Skills/Testimonials, and add two genuinely new content types:
  Articles (→ Blog) and Messages (contact submissions, admin-only).

See `.claude/MIGRATION_PLAN.md`'s addendum for how the admin panel fits
into the migration order (it's step 10, done last).
