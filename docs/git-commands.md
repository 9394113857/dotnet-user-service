# ============================================================
# FINAL: MASTER -> MAIN -> PULL README -> COMMIT -> PUSH
# ============================================================

# 1. Go to the repository
cd /e/DevOps-Projects/dotnet-user-service

# 2. Confirm the configured remote
git remote -v

# 3. Rename local master branch to main
git branch -M main

# 4. Confirm we're now on main
git branch

# 5. Pull the existing GitHub README/initial commit
# The histories started separately, so allow them to merge.
git pull origin main --allow-unrelated-histories

# ============================================================
# IF YOU GET A MERGE CONFLICT:
# STOP HERE and show me the output.
# ============================================================

# 6. Stage the complete project
git add .

# 7. Check what is staged
git status

# 8. Commit the merged repository state
git commit -m "chore: merge initial repository files"

# 9. Verify recent history
git log --oneline --graph --decorate -5

# 10. Push main to GitHub
git push -u origin main

# 11. Final verification
git status