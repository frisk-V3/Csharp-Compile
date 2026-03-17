#!/bin/bash
set -e

VERSION=$1

if [ -z "$VERSION" ]; then
  echo "Usage: ./release.sh v1.0.0"
  exit 1
fi

git tag $VERSION
git push origin $VERSION

echo "🚀 Released $VERSION"
