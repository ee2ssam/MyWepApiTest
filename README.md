# MyWepApiTest (HelloWeb)

ASP.NET Core (.NET 10) 웹 애플리케이션 프로젝트입니다.

---

## 🚀 GitHub Actions 배포 (CI/CD)

본 저장소에는 GitHub Actions를 통한 자동 빌드 및 배포 파이프라인([`.github/workflows/deploy.yml`](.github/workflows/deploy.yml))이 설정되어 있습니다.

`main` 브랜치에 코드를 푸시하면 다음 작업이 자동으로 수행됩니다:

1. **자동 빌드 및 아티팩트 배포**:
   - .NET 10 SDK 환경에서 프로젝트 의존성 복원, 빌드 및 `dotnet publish` 실행
   - 빌드된 릴리즈 산출물이 GitHub Actions **Artifacts**(`HelloWeb-Release`)로 업로드되어 즉시 다운로드 가능

2. **Docker 컨테이너 이미지 자동 배포 (GitHub Packages / GHCR)**:
   - [Dockerfile](Dockerfile)을 기반으로 컨테이너 이미지를 빌드
   - GitHub Container Registry(`ghcr.io`)로 자동 배포
   - 배포된 이미지 실행 방법:
     ```bash
     docker pull ghcr.io/<사용자명>/mywepapitest:latest
     docker run -d -p 8080:8080 --name helloweb ghcr.io/<사용자명>/mywepapitest:latest
     ```

---

## 🛠 로컬 개발 및 실행

```bash
# 의존성 복원 및 빌드
dotnet build HelloWeb/HelloWeb.csproj

# 로컬 실행
dotnet run --project HelloWeb/HelloWeb.csproj
```
