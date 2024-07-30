use axum::{
    http::StatusCode,
    routing::{get, post},
    Json, Router,
};
use serde::{Deserialize, Serialize};

mod util;

#[tokio::main]
async fn main() {
    let app = Router::new()
        .route("/items", get(get_items))
        .route("/orders", post(place_order));

    let listener = tokio::net::TcpListener::bind("0.0.0.0:3000").await.unwrap();
    axum::serve(listener, app).await.unwrap();
}

async fn get_items() -> (StatusCode, Json<Vec<ItemDto>>) {
    let items = vec![];

    (StatusCode::OK, Json(items))
}

async fn place_order(Json(payload): Json<PlaceOrderPayload>) -> (StatusCode, Json<User>) {
    let user = User {
        id: 1337,
        username: payload.username,
    };

    (StatusCode::CREATED, Json(user))
}

#[derive(Serialize)]
struct ItemDto {
    id: String,
    name: String,
    image_url: Option<String>,
    price: u32,
}

#[derive(Deserialize)]
struct PlaceOrderPayload {
    username: String,
}

#[derive(Serialize)]
struct User {
    id: u64,
    username: String,
}
