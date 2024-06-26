SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: public; Type: SCHEMA; Schema: -; Owner: -
--

-- *not* creating schema, since initdb creates it


--
-- Name: insert_01_library_event_and_return_id(text, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.insert_01_library_event_and_return_id(event_in text, context_state_id uuid) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    inserted_id integer;
BEGIN
    INSERT INTO events_01_library(event, timestamp, context_state_id)
    VALUES(event_in::JSON, now(), context_state_id) RETURNING id INTO inserted_id;
    return inserted_id;

END;
$$;


--
-- Name: insert_01_wishlist_aggregate_event_and_return_id(text, uuid, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.insert_01_wishlist_aggregate_event_and_return_id(event_in text, aggregate_id uuid, aggregate_state_id uuid) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
inserted_id integer;
    event_id integer;
BEGIN
    event_id := insert_01_wishlist_event_and_return_id(event_in, aggregate_id, aggregate_state_id);

INSERT INTO aggregate_events_01_wishlist(aggregate_id, event_id, aggregate_state_id )
VALUES(aggregate_id, event_id, aggregate_state_id) RETURNING id INTO inserted_id;
return event_id;
END;
$$;


--
-- Name: insert_01_wishlist_event_and_return_id(text, uuid, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.insert_01_wishlist_event_and_return_id(event_in text, aggregate_id uuid, aggregate_state_id uuid) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
inserted_id integer;
BEGIN
INSERT INTO events_01_wishlist(event, aggregate_id, timestamp)
VALUES(event_in::JSON, aggregate_id, now()) RETURNING id INTO inserted_id;
return inserted_id;
END;
$$;


--
-- Name: set_classic_optimistic_lock_01_library(); Type: PROCEDURE; Schema: public; Owner: -
--

CREATE PROCEDURE public.set_classic_optimistic_lock_01_library()
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'context_events_01_library_context_state_id_unique') THEN
ALTER TABLE events_01_library
    ADD CONSTRAINT context_events_01_library_context_state_id_unique UNIQUE (context_state_id);
END IF;
END;
$$;


--
-- Name: set_classic_optimistic_lock_01_wishlist(); Type: PROCEDURE; Schema: public; Owner: -
--

CREATE PROCEDURE public.set_classic_optimistic_lock_01_wishlist()
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'aggregate_events_01_wishlist_aggregate_id_state_id_unique') THEN
ALTER TABLE aggregate_events_01_wishlist
    ADD CONSTRAINT aggregate_events_01_wishlist_aggregate_id_state_id_unique UNIQUE (aggregate_state_id);
END IF;
END;
$$;


--
-- Name: un_set_classic_optimistic_lock_01_library(); Type: PROCEDURE; Schema: public; Owner: -
--

CREATE PROCEDURE public.un_set_classic_optimistic_lock_01_library()
    LANGUAGE plpgsql
    AS $$
BEGIN
    ALTER TABLE events_01_library
    DROP CONSTRAINT IF EXISTS context_events_01_library_context_state_id_unique;
END;
$$;


--
-- Name: un_set_classic_optimistic_lock_01_wishlist(); Type: PROCEDURE; Schema: public; Owner: -
--

CREATE PROCEDURE public.un_set_classic_optimistic_lock_01_wishlist()
    LANGUAGE plpgsql
    AS $$
BEGIN
ALTER TABLE aggregate_events_01_wishlist
DROP CONSTRAINT IF EXISTS aggregate_events_01_wishlist_aggregate_id_state_id_unique;
    -- You can have more SQL statements as needed
END;
$$;


--
-- Name: aggregate_events_01_wishlist_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.aggregate_events_01_wishlist_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: aggregate_events_01_wishlist; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.aggregate_events_01_wishlist (
    id integer DEFAULT nextval('public.aggregate_events_01_wishlist_id_seq'::regclass) NOT NULL,
    aggregate_id uuid NOT NULL,
    aggregate_state_id uuid,
    event_id integer
);


--
-- Name: events_01_library; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.events_01_library (
    id integer NOT NULL,
    event json NOT NULL,
    published boolean DEFAULT false NOT NULL,
    kafkaoffset bigint,
    kafkapartition integer,
    context_state_id uuid,
    "timestamp" timestamp without time zone NOT NULL
);


--
-- Name: events_01_library_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public.events_01_library ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.events_01_library_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: events_01_wishlist; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.events_01_wishlist (
    id integer NOT NULL,
    aggregate_id uuid NOT NULL,
    event json NOT NULL,
    published boolean DEFAULT false NOT NULL,
    kafkaoffset bigint,
    kafkapartition integer,
    "timestamp" timestamp without time zone NOT NULL
);


--
-- Name: events_01_wishlist_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public.events_01_wishlist ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.events_01_wishlist_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: schema_migrations; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.schema_migrations (
    version character varying(128) NOT NULL
);


--
-- Name: snapshots_01_library_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.snapshots_01_library_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: snapshots_01_library; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.snapshots_01_library (
    id integer DEFAULT nextval('public.snapshots_01_library_id_seq'::regclass) NOT NULL,
    snapshot json NOT NULL,
    event_id integer NOT NULL,
    "timestamp" timestamp without time zone NOT NULL
);


--
-- Name: snapshots_01_wishlist_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.snapshots_01_wishlist_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: snapshots_01_wishlist; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.snapshots_01_wishlist (
    id integer DEFAULT nextval('public.snapshots_01_wishlist_id_seq'::regclass) NOT NULL,
    snapshot json NOT NULL,
    event_id integer,
    aggregate_id uuid NOT NULL,
    aggregate_state_id uuid,
    "timestamp" timestamp without time zone NOT NULL
);


--
-- Name: aggregate_events_01_wishlist aggregate_events_01_wishlist_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.aggregate_events_01_wishlist
    ADD CONSTRAINT aggregate_events_01_wishlist_pkey PRIMARY KEY (id);


--
-- Name: events_01_library events_library_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.events_01_library
    ADD CONSTRAINT events_library_pkey PRIMARY KEY (id);


--
-- Name: events_01_wishlist events_wishlist_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.events_01_wishlist
    ADD CONSTRAINT events_wishlist_pkey PRIMARY KEY (id);


--
-- Name: schema_migrations schema_migrations_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.schema_migrations
    ADD CONSTRAINT schema_migrations_pkey PRIMARY KEY (version);


--
-- Name: snapshots_01_library snapshots_library_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.snapshots_01_library
    ADD CONSTRAINT snapshots_library_pkey PRIMARY KEY (id);


--
-- Name: snapshots_01_wishlist snapshots_wishlist_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.snapshots_01_wishlist
    ADD CONSTRAINT snapshots_wishlist_pkey PRIMARY KEY (id);


--
-- Name: aggregate_events_01_wishlist aggregate_events_01_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.aggregate_events_01_wishlist
    ADD CONSTRAINT aggregate_events_01_fk FOREIGN KEY (event_id) REFERENCES public.events_01_wishlist(id) MATCH FULL ON DELETE CASCADE;


--
-- Name: snapshots_01_library event_01_library_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.snapshots_01_library
    ADD CONSTRAINT event_01_library_fk FOREIGN KEY (event_id) REFERENCES public.events_01_library(id) MATCH FULL ON DELETE CASCADE;


--
-- Name: snapshots_01_wishlist event_01_wishlist_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.snapshots_01_wishlist
    ADD CONSTRAINT event_01_wishlist_fk FOREIGN KEY (event_id) REFERENCES public.events_01_wishlist(id) MATCH FULL ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--


--
-- Dbmate schema migrations
--

INSERT INTO public.schema_migrations (version) VALUES
    ('20240323171122'),
    ('20240323171416'),
    ('20240324155210');
