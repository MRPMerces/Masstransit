using UnityEngine;

public class FinancialMarketController : MonoBehaviour {
    public static FinancialMarketController financialMarket { get; protected set; }

    public float interestRate;

    public void sellStocks(float persentageStocks, Player player) {
        player.update_companyOwnership(-persentageStocks);

        player.opereatingIncome((int)(persentageStocks * player.totalExpenditure / 100));
    }

    public void buyStocks(float persentageStocks, Player buyer, Player seller) {

        // Buying from the public
        if (buyer == seller) {
            if (persentageStocks > buyer.publicOwnership) {
                buyer.update_companyOwnership(buyer.publicOwnership);
            }
            /// error

            buyer.update_companyOwnership(persentageStocks);

            buyer.opereatingIncome((int)-(persentageStocks * buyer.totalExpenditure / 100));
            return;
        }

        // Buying from another player
        //else {
        //    buyer.update_companyOwnership(persentageStocks);

        //    buyer.opereatingIncome((int)-(persentageStocks * player.totalExpenditure / 100));
        //    return;
        //}

    }

    /// <summary>
    /// Function to take loans
    /// </summary>
    /// <param name="loanId"> Id on loan, determines size</param>
    /// <param name="player"> Player to take loan</param>
    public void takeLoan(int loanId, Player player) {
        switch (loanId) {
            case 1:
                if (player.loan1 != null) {
                    Debug.LogError("Loan1 is not emty");
                    return;
                }
                player.loan1 = new Loan(100000, interestRate);
                break;
            case 2:
                if (player.loan2 != null) {
                    Debug.LogError("Player.takeLoan Loan2 is not emty");
                    return;
                }
                player.loan2 = new Loan(250000, interestRate);
                break;
            case 3:
                if (player.loan3 != null) {
                    Debug.LogError("Loan3 is not emty");
                    return;
                }
                player.loan3 = new Loan(500000, interestRate);
                break;
            default:
                Debug.LogError("Loan" + loanId + " does not exist");
                return;
        }
    }

    public void repayLoan(int loanId, Player player) {
        switch (loanId) {
            case 1:
                player.opereatingIncome(player.loan1.size);
                player.loan1 = null;
                break;

            case 2:
                player.opereatingIncome(player.loan2.size);
                player.loan2 = null;
                break;

            case 3:
                player.opereatingIncome(player.loan3.size);
                player.loan3 = null;
                break;
            default:
                Debug.LogError("Loan" + loanId + " does not exist");
                return;
        }
    }

    // Start is called before the first frame update
    void Start() {
        financialMarket = this;
        SpeedController.speedController.RegisterDayTickCallback(manageLoans);
    }

    void manageLoans() {
        if (World.world.playerController.players.Count > 0) {
            foreach (Player player in World.world.playerController.players) {
                if (player.loan1 != null) {
                    player.opereatingIncome(player.loan1.size * player.loan1.interest / 365);
                }
                if (player.loan2 != null) {
                    player.opereatingIncome(player.loan2.size * player.loan2.interest / 365);
                }
                if (player.loan3 != null) {
                    player.opereatingIncome(player.loan3.size * player.loan3.interest / 365);
                }
            }
        }

        Player human = World.world.playerController.human;

        if (human.loan1 != null) {
            human.opereatingIncome(human.loan1.size * human.loan1.interest / 365);
        }
        if (human.loan2 != null) {
            human.opereatingIncome(human.loan2.size * human.loan2.interest / 365);
        }
        if (human.loan3 != null) {
            human.opereatingIncome(human.loan3.size * human.loan3.interest / 365);
        }
    }
}

public class Loan {

    public Loan(int size, float interestRate) {
        this.size = size;
        interest = (int)((size * interestRate) / 365);
    }
    public int size { get; protected set; }

    public int interest { get; protected set; }
}

// stockprice = player.totalExpenditure / 100