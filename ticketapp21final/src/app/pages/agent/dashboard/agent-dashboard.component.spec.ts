import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AgentDashboardComponent } from './agent-dashboard.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
describe('AgentDashboardComponent', () => {
  let f: ComponentFixture<AgentDashboardComponent>;
  beforeEach(async () => { await TestBed.configureTestingModule({ imports:[AgentDashboardComponent,RouterTestingModule,HttpClientTestingModule] }).compileComponents(); f = TestBed.createComponent(AgentDashboardComponent); f.detectChanges(); });
  it('should create', () => expect(f.componentInstance).toBeTruthy());
  it('summary null initially', () => expect(f.componentInstance.summary()).toBeNull());
});
