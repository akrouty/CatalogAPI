# TP1 — DevOps & Docker

## 1. Mesure et observation

### Résultats de docker stats

Commande utilisée :

`docker stats --no-stream catalog-compose-api catalog-compose-db`

| Mesure         | API               | SQL Server        |
| -------------- | ----------------- | ----------------- |
| CPU            | 0,02 %            | 2,79 %            |
| Mémoire        | 64,04 MiB         | 830,5 MiB         |
| Réseau (RX/TX) | 29,7 kB / 1,87 MB | 9,91 kB / 11,6 kB |
| PIDs           | 31                | 257               |

On remarque que SQL Server consomme beaucoup plus de mémoire et de CPU que l'API. C'est normal, car il doit gérer les données, les requêtes et la mémoire cache. L'API est plus légère.

### Pourquoi mesurer en DevOps ?

La mesure permet de surveiller les performances, de détecter les problèmes et d'améliorer le fonctionnement des applications.

Trois métriques intéressantes sont :

- Le taux de réussite des pipelines CI/CD.
- Le temps nécessaire pour déployer une application.
- Le taux d'erreurs HTTP 5xx.

### Ping de port et healthcheck

Un ping de port vérifie seulement si un service accepte des connexions. Il ne garantit pas que l'application fonctionne correctement.

Un healthcheck peut effectuer des vérifications plus précises. Dans notre TP, Docker Compose utilise `SELECT 1` pour vérifier SQL Server avant de lancer l'API. Notre endpoint `/health`, lui, retourne simplement un statut `Healthy`.

## 2. Automatisation et Lean

Le multi-stage build permet de séparer la compilation et l'exécution de l'application.

Nous utilisons le SDK .NET 8 pour compiler le projet, puis une image ASP.NET Runtime pour l'exécuter.

Cela évite d'installer les outils de compilation sur chaque machine de CI/CD. L'image finale est aussi plus légère, ce qui réduit l'espace disque et les transferts réseau. Le cache Docker évite également de refaire certaines étapes inutilement.

## 3. Sécurité

Dans le Dockerfile, `USER app` permet de lancer l'application sans utiliser l'utilisateur root.

En cas de faille de sécurité, les actions possibles pour un attaquant sont donc plus limitées. Cela permet de réduire les risques liés aux privilèges élevés.

## 4. Analyse des images Docker

Nous avons comparé les tailles des images avec `docker image ls`.

| Image                 | Disk Usage |
| --------------------- | ---------- |
| Image finale de l'API | 348 MB     |
| SDK .NET 8            | 1,24 GB    |

Notre image finale est environ **72 % plus petite** que l'image SDK.

Cette différence vient du multi-stage build : l'image finale contient uniquement le runtime et les fichiers nécessaires au fonctionnement de l'API, sans les outils de compilation.
