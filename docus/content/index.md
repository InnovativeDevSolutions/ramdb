---
seo:
  title: ArmaRAMDb - High-Performance In-Memory Database for Arma 3
  description: Experience next-level persistence in Arma 3 with ArmaRAMDb - an ultra-fast in-memory data store powered by C# .NET 8. Redis-like API with automatic persistence and backup.
---

::u-page-hero
#title
ArmaRAMDb

#description
Experience next-level persistence in Arma 3 with our mod powered by C# .NET 8.

This ultra-fast in-memory data store offers unparalleled performance and scalability for your Arma 3 gameplay data management needs.

#links
  :::u-button
  ---
  color: primary
  size: xl
  to: /getting-started/introduction
  trailing-icon: i-lucide-arrow-right
  ---
  Get started
  :::

  :::u-button
  ---
  color: neutral
  icon: simple-icons-github
  size: xl
  to: https://github.com/InnovativeDevSolutions/ramdb
  variant: outline
  ---
  Star on GitHub
  :::
::

::u-page-section
#title
Powerful features for Arma 3 persistence

#features
  :::u-page-feature
  ---
  icon: i-lucide-zap
  ---
  #title
  [Lightning Fast]{.text-primary} Performance
  
  #description
  In-memory data storage with C# .NET 8 and concurrent data structures. Experience minimal latency for all database operations in your Arma 3 missions.
  :::

  :::u-page-feature
  ---
  icon: i-lucide-database
  ---
  #title
  [Redis-Like]{.text-primary} API
  
  #description
  Familiar Redis-style commands for key-value, hash tables, and lists. Easy to learn, powerful to use with operations like SET, GET, HSET, LPUSH, and more.
  :::

  :::u-page-feature
  ---
  icon: i-lucide-save
  ---
  #title
  [Automatic Persistence]{.text-primary}
  
  #description
  GZip-compressed saves to disk with configurable automatic backups. Never lose your data with scheduled backups and backup rotation.
  :::

  :::u-page-feature
  ---
  icon: i-lucide-shield-check
  ---
  #title
  [Thread-Safe]{.text-primary} Operations
  
  #description
  Built with concurrent data structures ensuring data integrity in multiplayer environments. Safe for simultaneous access from multiple clients.
  :::

  :::u-page-feature
  ---
  icon: i-lucide-split
  ---
  #title
  [Smart Chunking]{.text-primary}
  
  #description
  Automatic data chunking for large payloads exceeding the 20KB buffer. Seamlessly handles large arrays, loadouts, and complex game data.
  :::

  :::u-page-feature
  ---
  icon: i-lucide-users
  ---
  #title
  [Context Awareness]{.text-primary}
  
  #description
  Player Steam ID integration with special placeholders for single-player and multiplayer modes. Easy per-player data management without manual key tracking.
  :::
::
