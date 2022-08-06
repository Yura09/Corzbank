import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Guid } from "guid-typescript";
import { CardModel } from "../models/card.model";
import { UserModel } from "../models/user.model";

@Injectable({
    providedIn: 'root',
})

export class CardService {

    constructor(private http: HttpClient) { }

    url = 'https://localhost:44361/cards';

    getCards() {
        return this.http.get(this.url)
    }

    getCard(id: Guid) {
        return this.http.get(this.url + '/' + id)
    }

    getCardsForUser(id:Guid){
        return this.http.get(this.url + '/user/' + id)
    }

    createCard(card: CardModel) {
        return this.http.post(this.url, card)
    }

    deleteCard(id: Guid) {
        return this.http.delete(this.url + '/' + id)
    }
}