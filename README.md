# MyNetGame Web App (Supabase + Render 배포)

ASP.NET Core (.NET 10) 웹 애플리케이션으로, **Supabase (PostgreSQL)**의 `user_tbl`을 조회·조작하는 **CRUD Web API**가 구현되어 있으며 **GitHub를 통해 Render (render.com)**에 자동 배포됩니다.

---

## ⚡ 1. Supabase 데이터베이스 연결 정보

- **Host**: `aws-0-ap-southeast-1.pooler.supabase.com`
- **Port**: `5432`
- **Database**: `postgres`
- **User**: `postgres.xqshsxxcennnqbxxxgjt`
- **Target Table**: `user_tbl`

### `user_tbl` 스키마 구조
- `user_id` (text, Primary Key)
- `password` (text)
- `mobile` (varchar(11))
- `level` (smallint)
- `health` (integer)
- `gold` (integer)
- `m_date` (date, 기본값: 현재 날짜)

---

## 📡 2. `user_tbl` CRUD Web API 엔드포인트

| HTTP Method | 엔드포인트 URL | 설명 | 요청 본문 (JSON) 예시 |
| :--- | :--- | :--- | :--- |
| **GET** | `/api/user` | 전체 사용자 목록 조회 (SELECT) | 없음 |
| **GET** | `/api/user/{user_id}` | 특정 사용자 단건 조회 (SELECT) | 없음 |
| **POST** | `/api/user` | 신규 사용자 등록 (INSERT) | `{"user_id":"player1","password":"123","mobile":"01012345678","level":1,"health":100,"gold":500}` |
| **PUT** | `/api/user/{user_id}` | 사용자 정보 수정 (UPDATE) | `{"password":"new_pass","mobile":"01087654321","level":5,"health":150,"gold":1000}` |
| **DELETE** | `/api/user/{user_id}` | 사용자 삭제 (DELETE) | 없음 |

---

## 🚀 3. Render (render.com) 배포 설정

이 프로젝트는 Dockerfile을 기반으로 Render에서 무료로 호스팅됩니다:

1. [Render](https://render.com) 대시보드 ➡️ **+ New ➡️ Web Service** 선택
2. GitHub 저장소(`MyWepApiTest`) 선택:
   - **Environment**: `Docker`
   - **Dockerfile Path**: `./Dockerfile`
   - **Plan**: `Free`
   - **Region**: `Singapore`
3. **Environment Variables**:
   - `ConnectionStrings__DefaultConnection`:
     `Host=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.xqshsxxcennnqbxxxgjt;Password=MbcAcademy0420;SSL Mode=Require;Trust Server Certificate=true;`
   - `PORT`: `8080`
4. **Create Web Service** 클릭 시 자동 배포 완료

---

## 💻 4. 로컬 실행 및 테스트

```bash
dotnet run --project MyNetGame/MyNetGame.csproj
```
브라우저에서 `http://localhost:<포트>` 접속 시 웹 대시보드에서 `user_tbl`의 **등록(INSERT), 목록 조회(SELECT), 수정(UPDATE), 삭제(DELETE)**를 마우스 클릭으로 바로 테스트할 수 있습니다.
