-- migrate:up
GRANT ALL ON TABLE public.events_01_library TO safe;
GRANT ALL ON TABLE public.snapshots_01_library TO safe;
GRANT ALL ON SEQUENCE public.snapshots_01_library_id_seq TO safe;
GRANT ALL ON SEQUENCE public.events_01_library_id_seq TO safe;

GRANT postgres to safe;    -- dangerous zone!!!! This is to allow at applicative level the classic optimistic lock aggregateState check.


-- migrate:down

