# ArmaRAMDb Documentation

Official documentation site for ArmaRAMDb - High-Performance In-Memory Database for Arma 3.

## 🚀 Quick Start

```bash
# Install dependencies
npm install

# Start development server
npm run dev
```

Your documentation site will be running at `http://localhost:3000`

## 📁 Project Structure

```
docus/
├── content/                    # Documentation content
│   ├── index.md               # Homepage
│   ├── 1.getting-started/     # Getting started guides
│   │   ├── 1.introduction.md
│   │   ├── 2.installation.md
│   │   └── 3.quick-start.md
│   └── 2.api/                 # API Reference
│       ├── 1.core/            # Core functions
│       ├── 2.basic/           # Basic operations
│       ├── 3.hash/            # Hash operations
│       └── 4.list/            # List operations
├── public/                    # Static assets
├── app.config.ts              # Site configuration
├── nuxt.config.ts             # Nuxt configuration
├── package.json               # Dependencies
└── generate-docs.ps1          # Documentation generation script
```

## 🔄 Regenerating Documentation

If you make changes to the `docs/` folder, you can regenerate the Docus documentation:

```powershell
# From the ramdb root directory
.\docus\generate-docs.ps1
```

## 🚀 Deployment

The documentation is automatically deployed to GitHub Pages when changes are pushed to the `master` branch.

### Manual Build

To build locally:

```bash
# Build for production
npm run build

# Preview the build
npx serve .output/public
```

The built files will be in the `.output/public` directory.

### GitHub Pages

The site is automatically deployed via GitHub Actions to:
- **URL**: https://innovativedevsolutions.github.io/ramdb/
- **Workflow**: `.github/workflows/deploy-docs.yml`

## ⚡ Built With

- [Nuxt 4](https://nuxt.com) - The web framework
- [Nuxt Content](https://content.nuxt.com/) - File-based CMS
- [Nuxt UI](https://ui.nuxt.com) - UI components
- [Docus](https://docus.dev) - Documentation theme

## 📝 Writing Documentation

### File Naming Convention

Files are prefixed with numbers to control navigation order:
- `1.introduction.md` - First item
- `2.installation.md` - Second item
- etc.

### Frontmatter

Each markdown file should have frontmatter:

```yaml
---
title: Page Title
description: Short description for SEO
---
```

### Components

Docus provides special components:

#### Alerts

```markdown
::u-alert
---
color: primary
variant: subtle
icon: i-lucide-info
---
Your alert message here
::
```

#### Accordions

```markdown
::u-accordion
---
items:
  - label: "Question"
    content: "Answer"
    defaultOpen: false
---
::
```

### Navigation Files

Each directory should have a `.navigation.yml` file:

```yaml
title: Section Name
```

## 🔗 Links

- [Documentation Site](https://innovativedevsolutions.github.io/ramdb/)
- [GitHub Repository](https://github.com/InnovativeDevSolutions/ramdb)
- [Docus Documentation](https://docus.dev)

## 📄 License

This documentation is licensed under the Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.
