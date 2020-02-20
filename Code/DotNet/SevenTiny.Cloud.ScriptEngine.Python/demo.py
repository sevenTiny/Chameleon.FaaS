s = """
def vvv():
    person = ['张三', '李四']

    for p in person:
        print('name=', p)

    return 1

va = vvv()
"""

c = compile(s, '<string>', 'exec')

if c is not None:
    print('compile succeed!')

exec(c, globals(), locals())

a = globals()

b = locals()

d = a['va']

print(str(d))

