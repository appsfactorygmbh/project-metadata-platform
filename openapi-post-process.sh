file="$1"
yarn lint --no-inline-config "$file" && yarn prettier --write "$file"

