CREATE TYPE _coordinates AS (
    x float,
    y float,
    z float
);

CREATE DOMAIN coordinates AS _coordinates CHECK (
    (value).x IS NOT NULL
        AND (value).y IS NOT NULL
);

CREATE TYPE _addressarea as (
    name varchar(128),
    sub_area varchar(128)
);

CREATE DOMAIN addressarea AS _addressarea CHECK (
    (value).name IS NOT NULL
);

CREATE TYPE _addresshousingplot AS (
    plot_number integer,
    room_number integer
);

CREATE DOMAIN addresshousingplot AS _addresshousingplot CHECK (
    (value).plot_number >= 1
        AND ((value).room_number IS NULL OR (value).room_number >= 1)
);

CREATE TYPE _addresshousingapartment AS (
    -- NULL for lobby
    room_number integer
);

CREATE DOMAIN addresshousingapartment AS _addresshousingapartment CHECK (
    (value).room_number IS NULL
        OR (value).room_number >= 1
);

CREATE TYPE _addresshousingbuilding AS (
    plot addresshousingplot,
    apartment addresshousingapartment
);

CREATE DOMAIN addresshousingbuilding AS _addresshousingbuilding CHECK (
    ((value).plot IS NULL) <> ((value).apartment IS NULL)
);

CREATE TYPE _addresshousing AS (
    building addresshousingbuilding,
    ward integer,
    is_subdivision boolean
);

CREATE DOMAIN addresshousing AS _addresshousing CHECK (
    (value).ward IS NOT NULL
        AND (value).is_subdivision IS NOT NULL
);

CREATE TYPE _address AS (
    coordinates coordinates,
    area addressarea,
    housing addresshousing,
    map varchar(128),
    zone varchar(128),
    region varchar(128)
);

CREATE DOMAIN address AS _address CHECK (
    (value).coordinates IS NOT NULL
        AND (value).zone IS NOT NULL
        AND (value).region IS NOT NULL
);

CREATE TYPE orderstatus AS ENUM ('placed', 'dispatched', 'completed', 'cancelled');

CREATE TABLE item (
    id SERIAL PRIMARY KEY,
    external_id char(12) NOT NULL UNIQUE,
    name varchar(128) NOT NULL,
    image_url varchar(128),
    price integer NOT NULL CHECK (price >= 0),

    created_at timestamp NOT NULL DEFAULT current_timestamp,
    updated_at timestamp NOT NULL DEFAULT current_timestamp
);

CREATE INDEX item_by_external_id ON item USING HASH (external_id);

CREATE TABLE order_details (
    id SERIAL PRIMARY KEY,
    external_id char(12) NOT NULL UNIQUE,
    status orderstatus NOT NULL DEFAULT 'placed',
    customer_lodestone_id integer NOT NULL,
    customer_first_name varchar(15) NOT NULL,
    customer_last_name varchar(15) NOT NULL,
    customer_address address NOT NULL,
    customer_world varchar(32) NOT NULL,
    customer_datacenter varchar(32) NOT NULL,

    created_at timestamp NOT NULL DEFAULT current_timestamp,
    updated_at timestamp NOT NULL DEFAULT current_timestamp
);

CREATE INDEX order_details_by_external_id ON order_details USING HASH (external_id);

CREATE TABLE order_item (
    order_details SERIAL REFERENCES order_details,
    item SERIAL REFERENCES item,
    quantity integer NOT NULL CHECK (quantity >= 0),

    PRIMARY KEY (order_details, item)
);
