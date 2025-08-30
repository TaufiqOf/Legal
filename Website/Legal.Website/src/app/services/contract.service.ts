import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ApiService } from './api.service';
import { Contract, ContractParameterModel, PagedResponse, GetItemsParameterModel, IdParameterModel } from '../models/contract.model';

@Injectable({
  providedIn: 'root'
})
export class ContractService {
  private readonly moduleName = 'ADMIN';

  constructor(private apiService: ApiService) {}

  private unwrap<T>(raw: any): T {
    // Backend returns either { data: ... } or { result: ... }
    return (raw?.data ?? raw?.result ?? raw) as T;
  }

  getContracts(pageNumber: number, pageSize: number): Observable<PagedResponse<Contract>> {
    const parameter: GetItemsParameterModel = {
      pageNumber,
      pageSize,
      moduleName: 0 // ADMIN module
    };

    return this.apiService.executeQuery<any>(
      this.moduleName,
      'GetByPagedContractQuery',
      parameter
    ).pipe(
      map(raw => this.unwrap<any>(raw)),
      map(paged => {
        // Normalize property casing
        const inner = paged?.data ?? paged?.Data ? paged : paged?.result ?? paged;
        const data = inner.data ?? inner.Data ?? [];
        return {
          data: data as Contract[],
          count: inner.count ?? inner.Count ?? data.length ?? 0,
            pageNumber: inner.pageNumber ?? inner.PageNumber ?? 1,
            pageSize: inner.pageSize ?? inner.PageSize ?? pageSize,
            totalPage: inner.totalPage ?? inner.TotalPage ?? 0
        } as PagedResponse<Contract>;
      })
    );
  }

  getContract(id: string): Observable<Contract> {
    const parameter: IdParameterModel = {
      id,
      moduleName: 0 // ADMIN module
    };

    return this.apiService.executeQuery<any>(
      this.moduleName,
      'GetContractQuery',
      parameter
    ).pipe(
      map(raw => this.unwrap<any>(raw)),
      map(contract => contract as Contract)
    );
  }

  saveContract(contract: Contract): Observable<Contract> {
    const parameter: ContractParameterModel = {
      id: contract.id,
      author: contract.author,
      name: contract.name,
      description: contract.description,
      created: contract.created,
      updated: contract.updated,
      moduleName: 0 // ADMIN module
    };

    return this.apiService.executeCommand<any>(
      this.moduleName,
      'SaveContractCommand',
      parameter
    ).pipe(
      map(raw => this.unwrap<any>(raw)),
      map(saved => saved as Contract)
    );
  }

  deleteContract(id: string): Observable<boolean> {
    const parameter: IdParameterModel = {
      id,
      moduleName: 0 // ADMIN module
    };

    return this.apiService.executeCommand<any>(
      this.moduleName,
      'DeleteContractCommand',
      parameter
    ).pipe(
      map(raw => {
        // For delete we only care about success flag; raw may have success or result
        if (typeof raw?.success === 'boolean') return raw.success;
        if (typeof raw?.Success === 'boolean') return raw.Success;
        // If handler returns result true/false
        const unwrapped = this.unwrap<any>(raw);
        if (typeof unwrapped === 'boolean') return unwrapped;
        return false;
      })
    );
  }
}
