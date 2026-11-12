# MyNetGame Web App (Supabase + Render 배포)

ASP.NET Core (.NET 10) 웹 애플리케이션으로, **Supabase (PostgreSQL)** 데이터베이스를 조회·연동하고 **GitHub를 통해 Render (render.com)**에 자동 배포됩니다.

---

## ⚡ 1. Supabase 데이터베이스 연결 설정

`Npgsql` 및 `Dapper`를 통해 Supabase (PostgreSQL)에 안전하게 연결하여 데이터를 조회합니다.

### 연결 문자열 확인 방법 (Supabase 대시보드)
1. [Supabase](https://supabase.com) 프로젝트 대시보드 접속
2. 좌측 메뉴 **Project Settings** (톱니바퀴) > **Database** 이동
3. **Connection string** 섹션에서 정보 확인:
   ```text
   Host=aws-0-ap-northeast-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.<프로젝트ID>;Password=<비밀번호>;SSL Mode=Require;Trust Server Certificate=true;
   ```
4. 로컬 실행 시 [`MyNetGame/appsettings.json`](MyNetGame/appsettings.json)의 `DefaultConnection`에 등록되어 있습니다.

---

## 🚀 2. Render (render.com) 배포 방법

저장소 루트의 [`Dockerfile`](Dockerfile) 및 [`render.yaml`](render.yaml)을 통해 Render에서 무료로 호스팅됩니다.

### 단계별 배포 순서
1. **[render.com](https://render.com)에 로그인** (GitHub 계정 연동)
2. 대시보드 우측 상단 **+ New** ➡️ **Web Service** 선택
3. GitHub 저장소(`MyWepApiTest`)를 선택합니다:
   - **Name**: `mynetgame`
   - **Environment**: `Docker`
   - **Dockerfile Path**: `./Dockerfile`
   - **Plan**: `Free`
   - **Region**: `Singapore` (권장)
4. **Environment Variables (환경 변수) 설정**:
   - `ConnectionStrings__DefaultConnection` : 본인의 Supabase 연결 문자열
   - `PORT` : `8080`
5. **Create Web Service** 클릭!
   - Render가 Dockerfile을 통해 자동으로 컨테이너를 빌드하고 무료 도메인(`https://<서비스명>.onrender.com`)으로 배포합니다.
   - 웹 접속 시 메인 페이지에서 Supabase 연결 상태 및 테이블 조회를 실시간으로 확인할 수 있습니다.

---

## 🔄 3. GitHub Actions CI/CD 파이프라인 ([.github/workflows/deploy.yml](.github/workflows/deploy.yml))

`main` 브랜치에 코드를 푸시하면 다음 단계가 자동으로 실행됩니다:
1. **.NET 10 빌드 및 Publish**: `MyNetGame` 컴파일 검증 및 배포 산출물 아티팩트(`MyNetGame-Release`) 생성
2. **Docker 이미지 빌드 & GHCR 푸시**: GitHub Container Registry에 이미지 배포
3. **Render 배포 트리거 (`deploy-render`)**: Render Deploy Hook을 통해 자동 배포 요청

---

## 💻 로컬 개발 및 실행

```bash
# 의존성 복원 및 빌드
dotnet build MyNetGame/MyNetGame.csproj

# 로컬 실행
dotnet run --project MyNetGame/MyNetGame.csproj
```
