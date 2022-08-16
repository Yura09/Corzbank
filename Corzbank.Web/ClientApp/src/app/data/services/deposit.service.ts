import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Guid } from "guid-typescript";
import { Observable } from "rxjs";
import { Deposit } from "../dtos/deposit.dto";
import { ConfirmationModel } from "../models/confirmation.model";
import { DepositModel } from "../models/deposit.model";

@Injectable({
	providedIn: "root",
})
export class DepositService {

	private url = this.baseUrl + "deposits/";

	constructor(
		private http: HttpClient,
		@Inject("BASE_API_URL") private baseUrl: string
	) { }

	getDeposits(): Observable<Deposit[]> {
		return this.http.get<Deposit[]>(this.url);
	}

	getDeposit(id: Guid): Observable<Deposit> {
		return this.http.get<Deposit>(this.url + id);
	}

	getDepositsForUser(id: Guid): Observable<Deposit[]> {
		return this.http.get<Deposit[]>(this.url + "users/" + id);
	}

	openDeposit(deposit: DepositModel): Observable<Deposit> {
		return this.http.post<Deposit>(this.url, deposit);
	}

	closeDeposit(id: Guid): Observable<boolean> {
		return this.http.delete<boolean>(this.url + id);
	}

	confirmClosingDeposit(confirmationModel: ConfirmationModel): Observable<boolean> {
		return this.http.post<boolean>(this.url + "confirm-closing/", confirmationModel);
	}
}
