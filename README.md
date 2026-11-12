# MyNetGame Web App (Supabase + Render 배포)

ASP.NET Core (.NET 10) 웹 애플리케이션으로, **Supabase (PostgreSQL)**의 `user_tbl`을 조회·조작하는 **CRUD Web API**와 **저장 프로시저(`sp_login`) 기반 로그인 프로토콜 API**가 구현되어 있으며 **GitHub를 통해 Render (render.com)**에 자동 배포됩니다.

---

## ⚡ 1. Supabase 데이터베이스 연결 정보

- **Host**: `aws-0-ap-southeast-1.pooler.supabase.com`
- **Port**: `5432`
- **Database**: `postgres`
- **User**: `postgres.xqshsxxcennnqbxxxgjt`
- **Target Table**: `user_tbl`
- **Stored Procedure (Function)**: `sp_login(p_user_id TEXT, p_password TEXT) RETURNS INTEGER`

---

## 🔐 2. 로그인 프로토콜 Web API (Protocol 1101)

- **서비스 URL**: `POST /api/login` (또는 `POST /api/user/login`)
- **호출 방식**: `POST` (JSON)

### 1) Request Parameter (요청)
| 순번 | 변수명 | 변수타입 | 길이 | 필수 | 설명 / 비고 |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 1 | `Protocol` | Integer | - | 필수 | `1101` |
| 2 | `UserId` | String | 8 | 필수 | 유저 아이디 (최대 8자) |
| 3 | `Password` | String | 20 | 필수 | 비밀번호 (최대 20자) |

```json
{
  "protocol": 1101,
  "userId": "hero01",
  "password": "p@ss1234"
}
```

### 2) Response Parameter (응답)
| 순번 | 변수명 | 변수타입 | 설명 / 비고 |
| :--- | :--- | :--- | :--- |
| 1 | `Protocol` | Integer | `-1101` |
| 2 | `Result` | Integer | **결과 상태 코드**<br/>• `0`: 로그인 성공<br/>• `1`: 유저 아이디가 없음<br/>• `2`: 로그인 실패 (비밀번호 불일치) |
| 3 | `UserId` | String | 유저 아이디 |

```json
{
  "protocol": -1101,
  "result": 0,
  "userId": "hero01"
}
```

---

## 🛠 3. Supabase 저장 프로시저 (sp_login.sql)

Supabase PostgreSQL에 적용된 저장 함수 DDL:

```sql
CREATE OR REPLACE FUNCTION sp_login(
    p_user_id TEXT,
    p_password TEXT
)
RETURNS INTEGER
LANGUAGE plpgsql
AS $$
DECLARE
    v_db_password TEXT;
BEGIN
    SELECT password INTO v_db_password
    FROM user_tbl
    WHERE user_id = p_user_id;

    IF NOT FOUND THEN
        RETURN 1; -- 1: 유저 아이디가 없음
    END IF;

    IF v_db_password = p_password THEN
        RETURN 0; -- 0: 로그인 성공
    ELSE
        RETURN 2; -- 2: 로그인 실패 (비밀번호 불일치)
    END IF;
END;
$$;
```

---

## 📡 4. `user_tbl` CRUD Web API 엔드포인트

| HTTP Method | 엔드포인트 URL | 설명 | 요청 본문 (JSON) 예시 |
| :--- | :--- | :--- | :--- |
| **GET** | `/api/user` | 전체 사용자 목록 조회 (SELECT) | 없음 |
| **GET** | `/api/user/{user_id}` | 특정 사용자 단건 조회 (SELECT) | 없음 |
| **POST** | `/api/user` | 신규 사용자 등록 (INSERT) | `{"user_id":"hero01","password":"123","mobile":"01011112222","level":1,"health":100,"gold":500}` |
| **PUT** | `/api/user/{user_id}` | 사용자 정보 수정 (UPDATE) | `{"password":"new_pass","mobile":"01087654321","level":5,"health":150,"gold":1000}` |
| **DELETE** | `/api/user/{user_id}` | 사용자 삭제 (DELETE) | 없음 |

---

## 🚀 5. Render (render.com) 배포 및 로컬 실행

### 로컬 실행:
```bash
dotnet run --project MyNetGame/MyNetGame.csproj
```
브라우저에서 `http://localhost:<포트>` 접속 시 웹 대시보드에서 **로그인 프로토콜(1101)** 테스트 및 `user_tbl` CRUD를 마우스 클릭으로 바로 테스트할 수 있습니다.
