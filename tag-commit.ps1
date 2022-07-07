param (
    $tag = "v1.0.1"
)

git pull
git checkout main
git tag $tag
git push origin $tag
