import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ContractService } from '../../services/contract.service';
import { Contract, PagedResponse } from '../../models/contract.model';

@Component({
  selector: 'app-contract-list',
  templateUrl: './contract-list.component.html',
  styleUrls: ['./contract-list.component.css'],
  standalone: false
})
export class ContractListComponent implements OnInit {
  contracts: Contract[] = [];
  loading = false;
  error = '';
  
  // Pagination
  currentPage = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 0;

  constructor(
    private contractService: ContractService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadContracts();
  }

  loadContracts(): void {
    this.loading = true;
    this.error = '';

    this.contractService.getContracts(this.currentPage, this.pageSize).subscribe({
      next: (response: PagedResponse<Contract>) => {
        this.contracts = response.data;
        this.totalCount = response.count;
        this.totalPages = response.totalPage;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load contracts';
        this.loading = false;
        console.error('Error loading contracts:', error);
      }
    });
  }

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadContracts();
    }
  }

  editContract(contract: Contract): void {
    this.router.navigate(['/contracts/edit', contract.id]);
  }

  viewContract(contract: Contract): void {
    this.router.navigate(['/contracts/view', contract.id]);
  }

  deleteContract(contract: Contract): void {
    if (confirm(`Are you sure you want to delete the contract "${contract.name}"?`)) {
      this.contractService.deleteContract(contract.id).subscribe({
        next: (success) => {
          if (success) {
            this.loadContracts(); // Reload the list
          } else {
            this.error = 'Failed to delete contract';
          }
        },
        error: (error) => {
          this.error = 'Failed to delete contract';
          console.error('Error deleting contract:', error);
        }
      });
    }
  }

  createNewContract(): void {
    this.router.navigate(['/contracts/create']);
  }

  getPaginationArray(): number[] {
    const pages: number[] = [];
    const startPage = Math.max(1, this.currentPage - 2);
    const endPage = Math.min(this.totalPages, this.currentPage + 2);
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    
    return pages;
  }
}

