# GitHub Pages Setup Guide for ArmaRAMDb Documentation

This guide explains how to set up and deploy the ArmaRAMDb documentation to GitHub Pages.

## Prerequisites

- GitHub repository: `InnovativeDevSolutions/ramdb`
- Node.js 20+ installed locally
- Git configured

## Initial Setup (One-Time)

### 1. Enable GitHub Pages

1. Go to your repository on GitHub
2. Navigate to **Settings** → **Pages**
3. Under **Source**, select **GitHub Actions**
4. Save the settings

### 2. Install Dependencies

```bash
cd docus
npm install
```

### 3. Test Locally

```bash
# Development server
npm run dev

# Build for production
npm run build
```

Visit `http://localhost:3000` to preview the documentation.

## Project Structure

```
ramdb/
├── .github/
│   └── workflows/
│       └── deploy-docs.yml       # GitHub Actions workflow
├── docus/                        # Documentation site
│   ├── content/                  # Markdown documentation
│   │   ├── index.md              # Homepage
│   │   ├── 1.getting-started/    # Getting Started section
│   │   └── 2.api/                # API Reference
│   │       ├── 1.core/
│   │       ├── 2.basic/
│   │       ├── 3.hash/
│   │       └── 4.list/
│   ├── public/                   # Static assets
│   ├── app.config.ts             # Site configuration
│   ├── nuxt.config.ts            # Nuxt/Docus configuration
│   ├── package.json
│   └── generate-docs.ps1         # Script to regenerate docs from docs/
└── docs/                         # Original markdown docs
    ├── core/
    ├── basic/
    ├── hash/
    └── list/
```

## Deployment

### Automatic Deployment

The documentation is automatically deployed when you push changes to the `master` branch that affect:
- Any files in the `docus/` directory
- The workflow file `.github/workflows/deploy-docs.yml`

```bash
# Make changes to documentation
git add docus/
git commit -m "Update documentation"
git push origin master
```

The GitHub Actions workflow will:
1. Check out the repository
2. Install Node.js dependencies
3. Build the Nuxt/Docus site
4. Deploy to GitHub Pages

View the deployment progress at:
**Actions** tab in your GitHub repository

### Manual Deployment

You can also trigger a manual deployment:

1. Go to **Actions** tab in GitHub
2. Select **Deploy Documentation to GitHub Pages**
3. Click **Run workflow**
4. Select `master` branch
5. Click **Run workflow**

## Access the Documentation

Once deployed, your documentation will be available at:

**https://innovativedevsolutions.github.io/ramdb/**

## Updating Documentation

### Method 1: Edit Docus Files Directly

Edit the markdown files in `docus/content/`:

```bash
# Example: Update installation guide
notepad docus\content\1.getting-started\2.installation.md

# Commit and push
git add docus/content/
git commit -m "Update installation guide"
git push
```

### Method 2: Regenerate from docs/ Folder

If you update the original `docs/` folder, regenerate the Docus content:

```powershell
# Run the generation script
.\docus\generate-docs.ps1

# Commit and push
git add docus/content/
git commit -m "Regenerate documentation"
git push
```

## Configuration

### Site Configuration (app.config.ts)

```typescript
export default defineAppConfig({
  site: {
    name: 'ArmaRAMDb',
    description: 'High-Performance In-Memory Database for Arma 3',
    url: 'https://innovativedevsolutions.github.io/ramdb'
  },
  github: {
    url: 'https://github.com/InnovativeDevSolutions/ramdb',
    branch: 'master',
    rootDir: 'docus'
  }
})
```

### Build Configuration (nuxt.config.ts)

```typescript
export default defineNuxtConfig({
  extends: ['docus'],
  
  app: {
    baseURL: '/ramdb/',  // Important for GitHub Pages
    buildAssetsDir: '/assets/'
  },
  
  nitro: {
    preset: 'static'  // Static site generation
  }
})
```

## Troubleshooting

### Build Fails

**Error**: `npm ci` fails

**Solution**: Delete `package-lock.json` and `node_modules`, then run:
```bash
npm install
git add package-lock.json
git commit -m "Update package-lock.json"
git push
```

### Page Not Found (404)

**Error**: Documentation shows 404 after deployment

**Solution**: Check that `app.baseURL` in `nuxt.config.ts` matches your repository name:
```typescript
baseURL: '/ramdb/'  // Must match your repo name
```

### Assets Not Loading

**Error**: CSS/JS assets return 404

**Solution**: Verify `buildAssetsDir` is set correctly:
```typescript
buildAssetsDir: '/assets/'  // With leading slash
```

### Local Preview Doesn't Match Production

**Issue**: Site works locally but breaks on GitHub Pages

**Solution**: Test with production build:
```bash
NODE_ENV=production npm run build
npx serve .output/public
```

## Adding New Pages

### 1. Create Markdown File

```bash
# Example: Add a new troubleshooting guide
notepad docus\content\1.getting-started\4.troubleshooting.md
```

### 2. Add Frontmatter

```markdown
---
title: Troubleshooting
description: Common issues and solutions
---

# Troubleshooting

Your content here...
```

### 3. Update Navigation (Optional)

If you want custom navigation, edit `.navigation.yml` in the directory:

```yaml
title: Getting Started
```

### 4. Commit and Push

```bash
git add docus/content/1.getting-started/4.troubleshooting.md
git commit -m "Add troubleshooting guide"
git push
```

## Monitoring Deployments

### View Deployment Status

1. Go to repository **Actions** tab
2. Click on the latest workflow run
3. View logs for each step

### View Deployment History

1. Go to repository **Settings** → **Pages**
2. Scroll to **History** section
3. See all deployments and their status

### Rollback a Deployment

GitHub Pages doesn't have a built-in rollback, but you can:

1. Revert the commit that broke the site
2. Push the revert
3. Wait for automatic redeployment

```bash
git revert <commit-hash>
git push
```

## Best Practices

1. **Test Locally First**: Always run `npm run dev` and `npm run build` before pushing
2. **Write Good Commit Messages**: Makes tracking changes easier
3. **Use Branches**: For major documentation changes, use a branch and PR
4. **Keep Dependencies Updated**: Regularly update Nuxt and Docus
5. **Check Build Logs**: If deployment fails, check Actions logs
6. **Verify Links**: Test all internal links after major changes

## Additional Resources

- [Docus Documentation](https://docus.dev)
- [Nuxt Documentation](https://nuxt.com)
- [GitHub Pages Documentation](https://docs.github.com/en/pages)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)

## Support

If you encounter issues:

1. Check the [Troubleshooting](#troubleshooting) section
2. Review GitHub Actions logs
3. Open an issue on the repository

---

**Documentation Site**: https://innovativedevsolutions.github.io/ramdb/  
**Repository**: https://github.com/InnovativeDevSolutions/ramdb
