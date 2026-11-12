-- ==============================================================
-- 1. 유저등록 저장 프로시저 (sp_user_add, Protocol: 1102)
-- 결과 상태: 0: 등록 성공, 1: 중복 유저가 있음, 2: 등록 실패
-- ==============================================================
CREATE OR REPLACE FUNCTION sp_user_add(
    p_user_id TEXT,
    p_name TEXT,
    p_password TEXT
)
RETURNS INTEGER
LANGUAGE plpgsql
AS $$
BEGIN
    -- 중복 체크
    IF EXISTS (SELECT 1 FROM user_tbl WHERE user_id = p_user_id) THEN
        RETURN 1; -- 1: 중복 유저가 있음
    END IF;

    -- 신규 등록
    INSERT INTO user_tbl (user_id, name, password, level, health, gold, m_date)
    VALUES (p_user_id, p_name, p_password, 1, 100, 0, CURRENT_DATE);

    RETURN 0; -- 0: 등록 성공
EXCEPTION
    WHEN OTHERS THEN
        RETURN 2; -- 2: 등록 실패
END;
$$;


-- ==============================================================
-- 2. 플레이 유저 정보 저장 프로시저 (sp_user_info, Protocol: 1103)
-- 결과 상태: 0: 정보 가져오기 성공, 1: 정보 가져오기 실패
-- ==============================================================
CREATE OR REPLACE FUNCTION sp_user_info(
    p_user_id TEXT
)
RETURNS TABLE (
    result INTEGER,
    user_id TEXT,
    name TEXT,
    level INTEGER,
    health INTEGER,
    gold INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM user_tbl u WHERE u.user_id = p_user_id) THEN
        RETURN QUERY SELECT 1, p_user_id, ''::TEXT, 0, 0, 0; -- 1: 실패
        RETURN;
    END IF;

    RETURN QUERY
    SELECT 0, u.user_id, COALESCE(u.name, '')::TEXT, u.level::INTEGER, u.health::INTEGER, u.gold::INTEGER
    FROM user_tbl u
    WHERE u.user_id = p_user_id;
END;
$$;


-- ==============================================================
-- 3. 레벨업 저장 프로시저 (sp_level_up, Protocol: 1104)
-- 결과 상태: 0: 레벨업 성공, 1: 레벨업 실패
-- ==============================================================
CREATE OR REPLACE FUNCTION sp_level_up(
    p_user_id TEXT
)
RETURNS TABLE (
    result INTEGER,
    user_id TEXT,
    level INTEGER
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_new_level INTEGER;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM user_tbl u WHERE u.user_id = p_user_id) THEN
        RETURN QUERY SELECT 1, p_user_id, 0; -- 1: 실패
        RETURN;
    END IF;

    UPDATE user_tbl
    SET level = user_tbl.level + 1
    WHERE user_tbl.user_id = p_user_id
    RETURNING user_tbl.level INTO v_new_level;

    RETURN QUERY SELECT 0, p_user_id, v_new_level; -- 0: 성공
EXCEPTION
    WHEN OTHERS THEN
        RETURN QUERY SELECT 1, p_user_id, 0;
END;
$$;


-- ==============================================================
-- 4. 골드추가 저장 프로시저 (sp_add_gold, Protocol: 1105)
-- 결과 상태: 0: 추가 성공, 1: 추가 실패
-- ==============================================================
CREATE OR REPLACE FUNCTION sp_add_gold(
    p_user_id TEXT,
    p_gold INTEGER
)
RETURNS TABLE (
    result INTEGER,
    user_id TEXT,
    gold INTEGER
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_final_gold INTEGER;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM user_tbl u WHERE u.user_id = p_user_id) THEN
        RETURN QUERY SELECT 1, p_user_id, 0; -- 1: 실패
        RETURN;
    END IF;

    UPDATE user_tbl
    SET gold = user_tbl.gold + p_gold
    WHERE user_tbl.user_id = p_user_id
    RETURNING user_tbl.gold INTO v_final_gold;

    RETURN QUERY SELECT 0, p_user_id, v_final_gold; -- 0: 성공
EXCEPTION
    WHEN OTHERS THEN
        RETURN QUERY SELECT 1, p_user_id, 0;
END;
$$;
