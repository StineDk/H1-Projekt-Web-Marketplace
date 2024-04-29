CREATE TABLE item (
    itemid SERIAL PRIMARY KEY,
    type VARCHAR(255),
    gamename VARCHAR(255),
    genre VARCHAR(255),
    price INTEGER,
    manufacture VARCHAR(255),
    condition VARCHAR(255),
    description TEXT
);

CREATE TABLE users (id SERIAL PRIMARY KEY,  name text, phonenumber text, email text, city text);

ALTER TABLE items ADD user_id INTEGER,
ADD CONSTRAINT fk_user
FOREIGN KEY(user_id)
REFERENCES users(id)
ON DELETE CASCADE;

ALTER TABLE "public"."users"
ADD COLUMN "favorites" int[];
