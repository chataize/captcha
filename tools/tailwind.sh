#!/usr/bin/env bash
set -euo pipefail

cd ../ChatAIze.Captcha.Preview
tailwindcss -i ./wwwroot/css/app.css -o ./wwwroot/css/tailwind.css --watch
