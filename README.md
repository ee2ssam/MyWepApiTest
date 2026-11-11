# MyWepApiTest (HelloWeb)

ASP.NET Core (.NET 10) 웹 애플리케이션 프로젝트입니다.

---

## 🚀 Render (render.com) 배포 방법

이 프로젝트는 Dockerfile 및 [`render.yaml`](render.yaml) 청사진을 포함하고 있어 **Render**에서 무료로 손쉽게 배포할 수 있습니다.

### 방법 1. Render 대시보드에서 GitHub 저장소 연동 (가장 추천)
1. [Render](https://render.com)에 로그인하고 GitHub 계정을 연동합니다.
2. 대시보드 우측 상단 **+ New** 버튼을 누르고 **Web Service** (또는 **Blueprints**)를 선택합니다.
3. 이 저장소(`MyWepApiTest`)를 선택합니다:
   - **Environment**: `Docker`
   - **Dockerfile Path**: `./Dockerfile`
   - **Plan**: `Free`
   - **Region**: `Singapore` (권장)
4. **Create Web Service**를 클릭하면 Render가 Dockerfile을 기반으로 자동 빌드 및 배포를 완료하며 고유한 무료 도메인(`https://<서비스이름>.onrender.com`)을 제공합니다.
5. 이후 GitHub `main` 브랜치에 코드를 푸시할 때마다 Render가 자동으로 다시 빌드하여 배포합니다.

### 방법 2. GitHub Actions 자동 트리거 (Deploy Hook)
1. Render 웹 서비스 대시보드의 **Settings > Deploy Hook** 항목에서 Deploy Hook URL을 복사합니다.
2. GitHub 저장소의 **Settings > Secrets and variables > Actions**에서 새 시크릿을 등록합니다:
   - Name: `RENDER_DEPLOY_HOOK_URL`
   - Value: 복사한 Render Deploy Hook URL
3. 코드를 `main` 브랜치에 푸시하면 GitHub Actions의 `deploy-render` 작업이 Render에 배포를 자동 요청합니다.

---

## 🛠 GitHub Actions CI/CD 파이프라인 ([.github/workflows/deploy.yml](.github/workflows/deploy.yml))

`main` 브랜치에 푸시되면 다음 작업들이 자동으로 수행됩니다:

1. **빌드 & 패키징 (`build-and-publish`)**: .NET 10 빌드 후 릴리즈 산출물을 GitHub Actions Artifacts로 업로드
2. **Docker 이미지 빌드 & GHCR 푸시 (`docker-build-push`)**: GitHub Container Registry(`ghcr.io`)로 컨테이너 이미지 자동 배포
3. **Render 배포 트리거 (`deploy-render`)**: Render Deploy Hook을 통해 자동 배포 요청

---

## 💻 로컬 개발 및 실행

```bash
# 의존성 복원 및 빌드
dotnet build HelloWeb/HelloWeb.csproj

# 로컬 실행
dotnet run --project HelloWeb/HelloWeb.csproj
```
