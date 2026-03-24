import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AgentTicketDetailComponent } from './agent-ticket-detail.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';
describe('AgentTicketDetailComponent', () => {
  let f: ComponentFixture<AgentTicketDetailComponent>;
  beforeEach(async () => { await TestBed.configureTestingModule({ imports:[AgentTicketDetailComponent,RouterTestingModule,HttpClientTestingModule], providers:[{provide:ActivatedRoute,useValue:{snapshot:{paramMap:{get:()=>'7'}}}}] }).compileComponents(); f = TestBed.createComponent(AgentTicketDetailComponent); });
  it('should create', () => expect(f.componentInstance).toBeTruthy());
  it('tid resolves', () => expect(f.componentInstance.tid).toBe(7));
});
