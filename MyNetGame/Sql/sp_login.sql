-- ==============================================================
-- Supabase (PostgreSQL) 로그인 저장 함수 (Stored Procedure)
-- 프로토콜: 1101 로그인 처리
-- 반환값 (Result):
--   0: 로그인 성공
--   1: 유저 아이디가 없음
--   2: 로그인 실패 (비밀번호 불일치)
-- ==============================================================

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
    -- 1. 해당 유저의 비밀번호 조회
    SELECT password INTO v_db_password
    FROM user_tbl
    WHERE user_id = p_user_id;

    -- 유저가 존재하지 않는 경우
    IF NOT FOUND THEN
        RETURN 1; -- 1: 유저 아이디가 없음
    END IF;

    -- 2. 비밀번호 비교
    IF v_db_password = p_password THEN
        RETURN 0; -- 0: 로그인 성공
    ELSE
        RETURN 2; -- 2: 로그인 실패 (비밀번호 불일치)
    END IF;
END;
$$;

-- ==============================================================
-- 테스트 쿼리 예시
-- ==============================================================
-- SELECT sp_login('test_player01', 'password123!'); -- 성공 (0)
-- SELECT sp_login('test_player01', 'wrong_pass');    -- 비밀번호 불일치 (2)
-- SELECT sp_login('unknown_user', 'any');           -- 유저 없음 (1)
