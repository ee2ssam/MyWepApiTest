# MyFive Web App (Supabase + Render 배포)

ASP.NET Core (.NET 10) 웹 애플리케이션으로, **Supabase (PostgreSQL)** 데이터베이스와 연동되며 **GitHub를 통해 Render (render.com)**에 자동 배포됩니다.

---

## ⚡ 1. Supabase 데이터베이스 연결 설정

Supabase는 PostgreSQL 기반이므로 `Npgsql.EntityFrameworkCore.PostgreSQL`을 사용하여 연결됩니다.

### 연결 문자열 확인 방법 (Supabase 대시보드)
1. [Supabase](https://supabase.com) 프로젝트 대시보드 접속
2. 좌측 메뉴 **Project Settings** (톱니바퀴) > **Database** 이동
3. **Connection string** 섹션에서 **URI** 또는 **Connection Pooling** 선택
   - Transaction pooler (포트 6543) 또는 Direct (포트 5432)
4. 연결 문자열 형식 예시:
   ```text
   Host=aws-0-ap-northeast-2.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.<PROJECT-REF>;Password=<YOUR-PASSWORD>;SSL Mode=Require;Trust Server Certificate=true;
   ```

### 로컬 실행 시 연결 설정
[`MyFive/appsettings.json`](MyFive/appsettings.json)의 `DefaultConnection` 값을 본인의 Supabase 연결 정보로 수정합니다.

---

## 🚀 2. Render (render.com) 배포 방법

저장소 루트의 [`Dockerfile`](Dockerfile) 및 [`render.yaml`](render.yaml)을 통해 Render에서 무료로 호스팅됩니다.

### 단계별 배포 순서
1. **[render.com](https://render.com)에 로그인** (GitHub 계정 연동)
2. 대시보드 우측 상단 **+ New** ➡️ **Web Service** 선택
3. 본 GitHub 저장소(`MyWepApiTest`)를 선택합니다:
   - **Name**: `myfive` (원하는 이름)
   - **Environment**: `Docker`
   - **Dockerfile Path**: `./Dockerfile`
   - **Plan**: `Free`
   - **Region**: `Singapore` (권장)
4. **Environment Variables (환경 변수) 설정**:
   - `ConnectionStrings__DefaultConnection` : 본인의 Supabase 연결 문자열 입력
   - `PORT` : `8080`
5. **Create Web Service** 클릭!
   - Render가 Dockerfile을 이용해 빌드 및 배포를 완료합니다.
   - 배포 후 제공되는 `https://<서비스명>.onrender.com`에 접속하여 메인 화면의 **[Supabase 연결 확인]** 버튼으로 DB 정상 작동을 바로 테스트할 수 있습니다.

---

## 🔄 3. GitHub Actions CI/CD 파이프라인 ([.github/workflows/deploy.yml](.github/workflows/deploy.yml))

`main` 브랜치에 코드를 푸시하면 다음 단계가 자동으로 실행됩니다:
1. **.NET 10 빌드 및 Publish**: 컴파일 검증 및 배포 산출물 아티팩트(`MyFive-Release`) 생성
2. **Docker 이미지 빌드 & GHCR 푸시**: GitHub Container Registry에 이미지 배포
3. **Render 배포 트리거 (`deploy-render`)**:
   - Render의 **Deploy Hook** URL을 GitHub Secrets(`RENDER_DEPLOY_HOOK_URL`)에 등록해 두면 푸시 시 자동으로 Render 배포가 갱신됩니다.

---

## 💻 로컬 개발 및 실행

```bash
# 의존성 복원 및 빌드
dotnet build MyFive/MyFive.csproj

# 로컬 실행 (기본 포트 http://localhost:5000 등)
dotnet run --project MyFive/MyFive.csproj
```
