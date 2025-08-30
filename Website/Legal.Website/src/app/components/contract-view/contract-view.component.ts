import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ContractService } from '../../services/contract.service';
import { Contract } from '../../models/contract.model';

@Component({
  selector: 'app-contract-view',
  templateUrl: './contract-view.component.html',
  styleUrls: ['./contract-view.component.css'],
  standalone: false
})
export class ContractViewComponent implements OnInit {
  contract?: Contract;
  loading = false;
  error = '';

  constructor(
    private contractService: ContractService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.loadContract(params['id']);
      }
    });
  }

  private loadContract(id: string): void {
    this.loading = true;
    this.error = '';

    this.contractService.getContract(id).subscribe({
      next: (contract: Contract) => {
        this.contract = contract;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load contract';
        this.loading = false;
        console.error('Error loading contract:', error);
      }
    });
  }

  editContract(): void {
    if (this.contract) {
      this.router.navigate(['/contracts/edit', this.contract.id]);
    }
  }

  deleteContract(): void {
    if (this.contract && confirm(`Are you sure you want to delete the contract "${this.contract.name}"?`)) {
      this.contractService.deleteContract(this.contract.id).subscribe({
        next: (success) => {
          if (success) {
            this.router.navigate(['/contracts']);
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

  goBack(): void {
    this.router.navigate(['/contracts']);
  }
}
