@echo off
chcp 65001 >nul
echo ================================
echo Heroes of War - Git Push Script
echo ================================
echo.

cd /d "%~dp0"

echo [1/7] Initializing Git repository...
git init
if %errorlevel% neq 0 (
    echo Failed to initialize git. Make sure Git is installed.
    pause
    exit /b 1
)

echo.
echo [2/7] Configuring Git with your credentials...
git config --global user.name "fyl0453"
git config --global user.email "your-email@example.com"
git config --global credential.helper store

echo.
echo [3/7] Adding all files...
git add .

echo.
echo [4/7] Committing files...
git commit -m "Initial commit - Heroes of War MOBA Game"

echo.
echo [5/7] Setting up remote repository...
git remote remove origin 2>nul
git remote add origin https://github.com/fyl0453/MOBA.git

echo.
echo [6/7] Creating main branch...
git branch -M main

echo.
echo [7/7] Pushing to GitHub...
echo Note: You may need to enter your GitHub credentials when prompted
git push -u origin main --force

echo.
echo ================================
echo Push completed!
echo ================================
echo.
echo Repository URL: https://github.com/fyl0453/MOBA
echo.
pause
