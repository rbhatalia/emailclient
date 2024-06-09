# Email Client Application

## Problem Statement

Develop a core system for an email client with essential functionalities. This system will connect with user email accounts using Outlook (extendable to Gmail) and manage email data efficiently

## Database

+ The users when added to the platform are added, are saved in memory for this application as of now
+ Elasticsearch (Docker based) is used:

  + Email messages associated with each user address. (index name:- email_messages_index)
  + Mailbox details for each user address. (index name:- mailbox_details_index)

## Account creation

On application launch a user is presented a login screen, with field information

+ Common Name
+ EmailId

If the user is not in database, the user is added in the databased and his mailbox details are indexed

The user then needs to authenticate using OAUTH

After successful authentication, the user is redirected to a page where the user can see last 10 email update information

## Synchronization

A hosted timer service (Quartz.Net) which runs every minute to sync new changes from user email accounts to elatic search setup

## Frontend

A basic frontend which shows last 10 emails for the user logged in. A polling mechanism every 10 seconds (can be converted with web sockets) to sync new changes in the frontend
