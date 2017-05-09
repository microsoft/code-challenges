
# Load the quantmod library, so that we gain access to its functions

library(quantmod)

# We are comparing our performance against QQQ
# which tracks the Nasdaq 100 index. Run the lines of code
# below to get all the data from the beginning of the year.

start.date <- "2017-01-01"
QQQ <- getSymbols("QQQ", auto.assign = FALSE, from = start.date)

# View the contents of the data frame in variable explorer. Try:
#
# 1. Clicking on the chevron to the left of the QQQ variable to
#    drill down into its internal structure.
#
# 2. Clicking on the hourglass icon to the right of the QQQ 
#    variable to open up the data in the Data Table viewer.
#
# 3. Clicking on the Excel icon to the right of the QQQ variable
#    to open the data frame in Excel.

# Now lets plot QQQ for the YTD using the HighCharter package
# You should see a chart open up in the default browser, showing
# a plot of the Open, High, Low, and Close prices for each date.
# Note that the highcharter package is simply a wrapper around 
# the HighChart commercial library. See more at 
# https://highcharts.com.

library(highcharter)

# Notice the %>% operator. It's also known as the "pipe" operator
# which lets you chain together multiple function calls into a 
# single statement. It's provided by the magrittr library.

highchart(type = "stock") %>%
    hc_add_series(QQQ, type = "ohlc")

# Next, let's grab data for another stock, MSFT, and create a chart
# that compares the performance of MSFT vs. the QQQ S&P100 index.

MSFT <- getSymbols("MSFT", auto.assign = FALSE, from = start.date)

# Here, we generate a chart, showing both stocks head-to-head.
# However, because the stock prices are at different scales, it's 
# hard to see a clear analysis of their performance. Also
# notice how we've added the 538 (from 538.com) theme to the plot.

highchart(type = "stock") %>%
    hc_add_series(QQQ, type = "ohlc") %>%
    hc_add_series(MSFT) %>%
    hc_add_theme(hc_theme_538())

# So we need to transform our data so that we look at the increase
# in value of a portfolio invested equally between the two 
# series that we want to visualize. Before we can perform
# calculations, we first need to take a look at the data structures.

# Let's examine the data again in QQQ. You can see that it contains
# open, high, low, close, volume, and adjusted prices. The command
# below first converts QQQ to a data frame so that we can visualize
# it using our built in data grid viewer. Experiment with the grid
# to play with the different features that it offers!

View(as.data.frame(QQQ))

# We really are only interested in the adjusted prices at close 
# for the purposes of computing daily returns. We can extract the
# adjusted prices column from the time series using the Ad() 
# function. Note that in R, the "." is just part of the variable
# name; it has no semantic meaning.

QQQ.adjusted <- Ad(QQQ)

# Let's view the first few rows of the xts result set. We can see
# that it contains both dates and the adjusted price for QQQ. The
# dates are part of the row labels (each row can have a label)
# associated with it, and it's not a separate column. You can
# extract the row labels using the rownames() function, which
# we will see later in this lab.

View(as.data.frame(QQQ.adjusted))

# Now let's compute returns by buying a certain number of shares 
# of the stock

starting.investment = 100000

# Compute the number of shares we can buy by dividing starting 
# investment by opening price in the time period

MSFT.starting.shares = starting.investment / as.numeric(first(Op(MSFT)))
QQQ.starting.shares = starting.investment / as.numeric(first(Op(QQQ)))

# Now compute the daily adjusted market value of our shares
# This is a vector * scalar which yields another vector. Put 
# another way, we are multiplying each row in the adjusted closing
# price by the # shares that we hold.

MSFT.daily.market.value = Ad(MSFT) * MSFT.starting.shares
QQQ.daily.market.value = Ad(QQQ) * QQQ.starting.shares

# Now let's plot the value of our investments in QQQ vs. MSFT
# on a single chart to see which one wins.

highchart(type = "stock") %>%
    hc_add_series(QQQ.daily.market.value, name = "QQQ") %>%
    hc_add_series(MSFT.daily.market.value, name = "MSFT") %>%
    hc_add_theme(hc_theme_538())

# Now let's compute the market value based on the amount invested 
# in an arbitrary stock. We'll define a new function,
# compute.daily.market.value(), which takes a symbol, the amount
# invested in it and returns a list that contains the market
# value for each day in the investment range.

# There are some new notable R features in this example:
# 1. Functions are defined as values
# 2. The return statement returns the result of an expression. If
#    you don't use return, then the value of the last evaluated
#    expression will be returned from your function.

# When you run the function, you can either:
# 1. Select all the code in the editor and press CTRL+ENTER to 
#    send it in one chunk to the REPL.
# 2. Send one line at a time by repeatedly pressing CTRL+ENTER
#    to see our multi-line support in the REPL. You'll see this
#    in the "+" characters running down the left margin to 
#    indicate that you're continuing the current block of code
#    in the REPL.

compute.daily.market.value <- function(symbol, dollars) {
    symbol.data <- getSymbols(
        symbol,
        auto.assign = FALSE,
        from = start.date)
    shares <- dollars / first(Op(symbol.data))
    return(Ad(symbol.data) * shares)
}

# Now, let's get the daily returns, expressed as a percentage
# gain or loss, based on the adjusted prices. All we have to 
# do now is call the compute.daily.market.value() with the 
# name of the symbol, and the starting investment.

QQQ.daily.market.value <- compute.daily.market.value(
    "QQQ",
    starting.investment)

View(as.data.frame(QQQ.daily.market.value))

# Now let's do the same thing for an equal weight portfolio 
# of FANG (Facebook, Amazon, Netflix, Google). Let's define
# a data frame that contains two columns: symbols and percentage
# that represent the stock symbol names and their percentage
# weight in the portfolio (these ideally add up to 1.0 - 
# enforcing that is an exercise left to the reader).

# You will notice that there is a peculiar named parameter in
# the call to the data.frame() function to define the data frame.
# stringsAsFactors is something you'll run into often in R - 
# by default, R interprets character strings as Factors. These
# are roughly analogous to enumeration values in other languages.
# You get better performance, but these values are not treated
# as strings. This parameter forces the data.frame() function to
# treat the list of strings assigned to the symbols column as
# strings and not factors.

portfolio <- data.frame(
    symbols = c("FB", "AMZN", "NFLX", "GOOG"),
    percentage = c(0.25, 0.25, 0.25, 0.25),
    stringsAsFactors = FALSE)

# Now we need to change our daily market value function to compute 
# against portfolio of stocks rather than a single stock.

compute.portfolio.daily.market.value <- function(portfolio, dollars) {

    # The cbind() function binds two dataframes together by column. 
    # In this case, we're appending a new column, dollars, to
    # the portfolio dataframe. That column contains the result
    # of muliplying each row in the percentage column against the
    # dollars scalar value.

    # Note that there are no explicit loops here. In general, if
    # you find yourself writing a loop in R, you're probably
    # doing it wrong :)
    
    portfolio <- cbind(
        portfolio,
        dollars = portfolio$percentage * dollars)

    # We define a variable here in the middle of the body of
    # the compute.portfolio.daily.market.value() function. Because
    # R supports lexical scoping, this variable can be manipulated
    # from within the compute.daily.market.value() function that
    # is defined immediately below it.

    df <- NULL

    # This is a nested function. It can access variables in
    # the enclosing scope by using the <<- operator.

    compute.daily.market.value <- function(symbol, dollars) {

        symbol.data <- getSymbols(
            symbol,
            auto.assign = FALSE,
            from = start.date)

        shares <- dollars / as.numeric(first(Op(symbol.data)))
        market.value <- Ad(symbol.data) * shares

        # Here, we use the <<- operator to either:
        # 1. Assign a new data frame if one didn't exist already
        # 2. Append a new column to the existing data frame.

        if (is.null(df)) {
            df <<- data.frame(market.value)
        }
        else {
            df <<- cbind(df, data.frame(market.value))
        }
    }

    # This is how we iterate over all of the rows in the 
    # portfolio data frame. We pass the symbols column to
    # the compute.daily.market.value() function by dereferencing
    # it using the $ operator. Similarly, we pass the dollars
    # column as well. When this function returns, the df
    # variable will now hold a new data frame containing the
    # daily market values of all of the stocks in the portfolio.

    mapply(
        compute.daily.market.value,
        portfolio$symbols,
        portfolio$dollars)

    # Compute totals for each day using the rowSums function.
    # Here we create a new data frame that contains a column
    # called Total, which contains the sum of the daily
    # market values of each of the stock symbols.

    df <- cbind(
        df,
        data.frame(Total = rowSums(df)))

    return(df)
}

# Here's a single function call that computes the daily market
# value of our portfolio.

portfolio.daily.market.value <- compute.portfolio.daily.market.value(
    portfolio,
    starting.investment)

# Let's define another "portfolio", but this time it contains only
# a single stock - QQQ which represents the NASDAQ 100 index.

index <- data.frame(
    symbols = c("QQQ"),
    percentage = c(1.0),
    stringsAsFactors = FALSE)

index.daily.market.value <- compute.portfolio.daily.market.value(
    index,
    starting.investment)

# Our daily market value results are in a data frame.
# We need to convert them to an xts object (time series object)
# so that the dates display correctly in the plots. We didn't
# need to do this in our previous single-stock calculations 
# because we modified an existing xts object in-place. In our
# new portfolio base calculation, we are computing a new data 
# frame from scratch, hence the need to convert it to
# an xts object using the labels from the data frame that 
# contain the identity of each trading day.

convert.totals.dataframe.to.xts <- function(df) {
    return(
        xts(
            df$Total,
            as.POSIXct(rownames(df))))
}

# Plot the comparison chart between our portfolio and QQQ.

highchart(type = "stock") %>%
    hc_add_series(
        convert.totals.dataframe.to.xts(index.daily.market.value),
        name = "QQQ") %>%
    hc_add_series(
        convert.totals.dataframe.to.xts(portfolio.daily.market.value),
        name = "Portfolio") %>%
    hc_add_theme(hc_theme_538())

# Now the next step in this is to turn this analysis into an 
# application where you can pick the stocks to add to your 
# asset allocation and their percentages. See the next 
# section in the lab manual for how to complete this part of the lab.
