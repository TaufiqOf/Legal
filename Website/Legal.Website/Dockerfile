# Angular production build + nginx serve
FROM node:20-alpine AS build

WORKDIR /app

# Install build prerequisites only if needed by native deps
RUN apk add --no-cache git python3 make g++ bash

# Copy dependency manifests (Docker build context = repo root; Dockerfile in project folder)
COPY Website/Legal.Website/package*.json ./

# Install dependencies
RUN npm ci --no-audit --no-fund || (echo 'npm ci failed' && ls -l && exit 1)

# Copy remaining project sources
COPY Website/Legal.Website ./

# Diagnostics
RUN echo "Node:" && node -v && echo "NPM:" && npm -v && echo "Contents:" && ls -1

# Build (capture exit code and show tail of Angular output if it fails)
RUN set -e; \
    NODE_OPTIONS=--max_old_space_size=3072 npm run build:prod -- --verbose 2>&1 | tee build.log || BUILD_STATUS=$?; \
    if [ "${BUILD_STATUS:-0}" != "0" ]; then \
      echo '--- Angular build failed. Last 200 log lines ---'; \
      tail -n 200 build.log || true; \
      exit $BUILD_STATUS; \
    fi

# ---------------- Runtime Stage ----------------
FROM nginx:alpine AS runtime

# Copy custom nginx configuration
COPY Website/Legal.Website/nginx.conf /etc/nginx/nginx.conf

# Copy built app
COPY --from=build /app/dist/Legal.Website/browser /usr/share/nginx/html

EXPOSE 80
HEALTHCHECK --interval=30s --timeout=5s --retries=3 CMD wget -qO- http://127.0.0.1/health || exit 1

CMD ["nginx", "-g", "daemon off;"]
