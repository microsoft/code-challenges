
# Load the quantmod library, so that we gain access to its functions

library(quantmod)

# We are comparing our performance against QQQ
# which tracks the Nasdaq 100

start.date <- "2017-01-01"
QQQ <- getSymbols("QQQ", auto.assign = FALSE, from = start.date)

# View this in variable explorer
# Open table viewer
# Export to Excel

# Now lets plot QQQ for the YTD using the HighCharter package

library(highcharter)

highchart(type = "stock") %>%
    hc_add_series(QQQ, type = "ohlc")

# Let's get another stock MSFT and plot that in a comparison chart

MSFT <- getSymbols("MSFT", auto.assign = FALSE, from = start.date)

highchart(type = "stock") %>%
    hc_add_series(QQQ, type = "ohlc") %>%
    hc_add_series(MSFT) %>%
    hc_add_theme(hc_theme_538())

# Let's examine the data again in QQQ. You can see that it contains
# open, high, low, close, volume, and adjusted prices. 

View(as.data.frame(QQQ))

# We really are only interested in the adjusted prices at close 
# for the purposes of computing daily returns, which we can 
# extract using the Ad() function

QQQ.adjusted <- Ad(QQQ)

# Let's view the first few rows of the xts result set. We can see
# that it contains both dates and the adjusted price for QQQ.

View(as.data.frame(QQQ.adjusted))

# Now let's compute returns by buying a certain number of shares of the stock

starting.investment = 100000

# Compute the number of shares we can buy by dividing starting investment by opening price

MSFT.starting.shares = starting.investment / as.numeric(first(Op(MSFT)))
QQQ.starting.shares = starting.investment / as.numeric(first(Op(QQQ)))

# Now compute the daily adjusted book value of our shares
# This is a vector * scalar which yields another vector

MSFT.daily.book.value = Ad(MSFT) * MSFT.starting.shares
QQQ.daily.book.value = Ad(QQQ) * QQQ.starting.shares

highchart(type = "stock") %>%
    hc_add_series(QQQ.daily.book.value, name = "QQQ") %>%
    hc_add_series(MSFT.daily.book.value, name = "MSFT") %>%
    hc_add_theme(hc_theme_538())

# Now let's compute the book value based on the amount invested in a stock

compute.daily.book.value <- function(symbol, dollars) {
    symbol.data <- getSymbols(symbol, auto.assign = FALSE, from = start.date)
    shares <- dollars / first(Op(symbol.data))
    return(Ad(symbol.data) * shares)
}

# Now, let's get the daily returns, expressed as a percentage
# gain or loss, based on the adjusted prices

QQQ.daily.book.value <- compute.daily.book.value("QQQ", starting.investment)
View(as.data.frame(QQQ.daily.book.value))

# Let's test it on TSLA

TSLA.daily.book.value <- compute.daily.book.value("TSLA", starting.investment)
View(TSLA.daily.book.value)

TSLA.cumulative.returns <- cumsum(TSLA.daily.returns * starting.investment)
head(TSLA.cumulative.returns)

# Now let's do the same thing for an equal weight portfolio of FANG

portfolio <- data.frame(
    symbols = c("FB", "AMZN", "NFLX", "GOOG"),
    percentage = c(0.25, 0.25, 0.25, 0.25),
    stringsAsFactors = FALSE)

# We need to adjust our daily book value function to compute against portfolio of stocks

compute.portfolio.daily.book.value <- function(portfolio, dollars) {
    portfolio <- cbind(portfolio, dollars = portfolio$percentage * dollars)
    df <- NULL

    compute.daily.book.value <- function(symbol, dollars) {

        symbol.data <- getSymbols(
            symbol,
            auto.assign = FALSE,
            from = start.date)

        shares <- dollars / as.numeric(first(Op(symbol.data)))
        book.value <- Ad(symbol.data) * shares

        if (is.null(df)) {
            df <<- data.frame(book.value)
        }
        else {
            df <<- cbind(df, data.frame(book.value))
        }
    }

    mapply(compute.daily.book.value, portfolio$symbols, portfolio$dollars)

    # Compute totals for each day

    df <- cbind(df, data.frame(Total = rowSums(df)))
    return(df)
}

portfolio.daily.book.value <- compute.portfolio.daily.book.value(portfolio, starting.investment)

index <- data.frame(
    symbols = c("QQQ"),
    percentage = c(1.0),
    stringsAsFactors = FALSE)

index.daily.book.value <- compute.portfolio.daily.book.value(index, starting.investment)

# Now let's plot the two curves against each other
# This loses the date labels - better to remove columns?

convert.totals.dataframe.to.xts <- function(df) {
    return(xts(df$Total, as.POSIXct(rownames(df))))
}

highchart(type = "stock") %>%
    hc_add_series(convert.totals.dataframe.to.xts(index.daily.book.value), name = "QQQ") %>%
    hc_add_series(convert.totals.dataframe.to.xts(portfolio.daily.book.value), name = "Portfolio") %>%
    hc_add_theme(hc_theme_538())

# Now the next step in this is to turn this analysis into an applicatoin
# where you can pick the stocks to add to your asset allocation and their 
# proportions. The constraint is that they must add up to 100% in your allocation.
# Hit the compute button to generate the results.
