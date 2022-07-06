param (
    $tag = "v1.0.1"
)

git pull
git checkout main
git push origin $tag
