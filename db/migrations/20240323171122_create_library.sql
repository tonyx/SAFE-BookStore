-- migrate:up

CREATE TABLE public.events_01_library (
                                          id integer NOT NULL,
                                          event json NOT NULL,
                                          published boolean NOT NULL DEFAULT false,
                                          kafkaoffset BIGINT,
                                          kafkapartition INTEGER,
                                          context_state_id uuid,
                                          "timestamp" timestamp without time zone NOT NULL
);

ALTER TABLE public.events_01_library ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.events_01_library_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);

CREATE SEQUENCE public.snapshots_01_library_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

CREATE TABLE public.snapshots_01_library (
                                             id integer DEFAULT nextval('public.snapshots_01_library_id_seq'::regclass) NOT NULL,
                                             snapshot json NOT NULL,
                                             event_id integer NOT NULL,
                                             "timestamp" timestamp without time zone NOT NULL
);

ALTER TABLE ONLY public.events_01_library
    ADD CONSTRAINT events_library_pkey PRIMARY KEY (id);

ALTER TABLE ONLY public.snapshots_01_library
    ADD CONSTRAINT snapshots_library_pkey PRIMARY KEY (id);

ALTER TABLE ONLY public.snapshots_01_library
    ADD CONSTRAINT event_01_library_fk FOREIGN KEY (event_id) REFERENCES public.events_01_library(id) MATCH FULL ON DELETE CASCADE;


CREATE OR REPLACE FUNCTION insert_01_library_event_and_return_id(
    IN event_in TEXT,
    IN context_state_id uuid
)
RETURNS int

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

CREATE OR REPLACE PROCEDURE set_classic_optimistic_lock_01_library() AS $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'context_events_01_library_context_state_id_unique') THEN
ALTER TABLE events_01_library
    ADD CONSTRAINT context_events_01_library_context_state_id_unique UNIQUE (context_state_id);
END IF;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE un_set_classic_optimistic_lock_01_library() AS $$
BEGIN
    ALTER TABLE events_01_library
    DROP CONSTRAINT IF EXISTS context_events_01_library_context_state_id_unique;
END;
$$ LANGUAGE plpgsql;


-- migrate:down

