scoreboard players set #tmp tmp 0
execute as @e[type=armor_stand,tag=cell,limit=1,scores={tmp=0}] at @s run function slay:create_field/remove_cells/check_one

# tellraw @a ["",{"score":{"name":"#tmp","objective":"tmp"}}]
execute as @e[type=armor_stand,tag=cell,tag=checked] run scoreboard players operation @s tmp = #tmp tmp
tag @e[type=armor_stand,tag=cell,tag=checked] remove checked
execute if entity @e[type=armor_stand,tag=cell,limit=1,scores={tmp=0}] run function slay:create_field/remove_cells/check_multiple
# say check_multiple