import { ClaimComment } from './claim-comment.model';
import { ClaimAudit } from './claim-audit.model';

export interface Claim {
    id: number;
    policyNumber: string;
    claimNumber: string;
    claimDate: string;
    amount: number;
    description: string;
    statusCode: string;
    comments?: ClaimComment[];
    audits?: ClaimAudit[];
}
